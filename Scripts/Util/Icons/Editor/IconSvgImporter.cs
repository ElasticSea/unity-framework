using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ElasticSea.Framework.Scripts.Util.Icons
{
    public class IconSvgImportWindow : EditorWindow
    {
        private string svgPath;
        private string iconName;
        private IconExplorer explorer;

        public static void Open(IconExplorer explorer)
        {
            var path = EditorUtility.OpenFilePanel("Add outline SVG icon", "", "svg");
            if (string.IsNullOrEmpty(path)) return;
            var window = GetWindow<IconSvgImportWindow>(true, "Add SVG Icon");
            window.svgPath = path;
            window.iconName = Regex.Replace(Path.GetFileNameWithoutExtension(path).ToLowerInvariant(), "[^a-z0-9_]", "_");
            window.explorer = explorer;
            window.minSize = new Vector2(430, 170);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("SVG", svgPath ?? "");
            iconName = EditorGUILayout.TextField("Icon name", iconName);
            EditorGUILayout.HelpBox("Use solid, filled paths. Convert strokes and shapes to outlines and flatten transforms. The source SVG, permanent codepoint, font and TMP atlas are saved together.", MessageType.Info);
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(iconName) || !Regex.IsMatch(iconName, "^[a-z][a-z0-9_]*$")))
            {
                if (GUILayout.Button("Add and rebuild"))
                {
                    try
                    {
                        var pack = IconSvgImporter.Build(svgPath, iconName);
                        (explorer != null ? explorer : IconExplorer.Show()).SelectIcon(pack, iconName);
                        Close();
                    }
                    catch (Exception error)
                    {
                        Debug.LogException(error);
                        EditorUtility.DisplayDialog("SVG import failed", error.Message, "OK");
                    }
                }
            }
        }
    }

    public static class IconSvgImporter
    {
        public const string Root = "Assets/Custom Icons";
        public const string PackPath = Root + "/Custom Icons.asset";
        public const string FontPath = Root + "/CustomIcons.ttf";
        public const string TmpPath = Root + "/Custom Icons SDF.asset";
        private const string CodepointsPath = Root + "/CustomIcons.codepoints.txt";
        public static bool CanRebuild => File.Exists(Root + "/icons.json");

        public static void RebuildFromMenu(IconExplorer explorer)
        {
            try { explorer.SelectIcon(Build(), ""); }
            catch (Exception error)
            {
                Debug.LogException(error);
                EditorUtility.DisplayDialog("Icon rebuild failed", error.Message, "OK");
            }
        }

        // Public so build tooling and editor tests exercise the same path as Add SVG.
        public static IconFont Build(string svgPath = null, string iconName = null)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Exit Play Mode before rebuilding icons.");
            string work = Path.GetFullPath("Library/IconFontTools/build-" + Guid.NewGuid().ToString("N"));
            string stage = "Assets/__IconFontBuild_" + Guid.NewGuid().ToString("N");
            var before = new Dictionary<string, byte[]>();
            var created = new HashSet<string>();
            TMP_FontAsset candidate = null;
            bool committing = false;
            Directory.CreateDirectory(work);
            EditorApplication.LockReloadAssemblies();
            AssetDatabase.DisallowAutoRefresh();
            try
            {
                EditorUtility.DisplayProgressBar("SVG icon font", "Preparing font generator", 0.1f);
                string tools = FindTools();
                string python = EnsurePython(tools);
                var packs = AssetDatabase.FindAssets("t:IconFont").Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<IconFont>).Where(p => p != null).ToArray();
                var reserved = new HashSet<int>();
                foreach (var pack in packs.Where(p => AssetDatabase.GetAssetPath(p) != PackPath))
                    foreach (var code in ParseCodes(pack.CodePoints)) reserved.Add((int)code);
                foreach (var guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path == TmpPath) continue;
                    var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                    foreach (var character in font.characterTable)
                        if (character.unicode >= 0xE000 && character.unicode <= 0xF8FF)
                            reserved.Add((int)character.unicode);
                }
                string reservedPath = work + "/reserved.json";
                File.WriteAllText(reservedPath, "[" + string.Join(",", reserved.OrderBy(c => c)) + "]");
                string output = work + "/output";
                var args = new List<string> { tools + "/build_icons.py", "--source", Path.GetFullPath(Root), "--output", output, "--reserved", reservedPath };
                if (svgPath != null) args.AddRange(new[] { "--svg", Path.GetFullPath(svgPath), "--name", iconName });
                EditorUtility.DisplayProgressBar("SVG icon font", "Validating SVGs and generating outlines", 0.3f);
                Run(python, args.ToArray());

                // Import a temporary font and prove every glyph fits before touching live assets.
                EnsureFolder(stage);
                File.Copy(output + "/CustomIcons.ttf", stage + "/Candidate.ttf");
                AssetDatabase.ImportAsset(stage + "/Candidate.ttf", ImportAssetOptions.ForceSynchronousImport);
                var sourceFont = AssetDatabase.LoadAssetAtPath<Font>(stage + "/Candidate.ttf");
                if (sourceFont == null) throw new InvalidOperationException("Unity could not import the generated font.");
                var codes = ParseCodes(File.ReadAllText(output + "/CustomIcons.codepoints.txt")).ToArray();
                EditorUtility.DisplayProgressBar("SVG icon font", "Generating and checking the TMP atlas", 0.55f);
                candidate = TMP_FontAsset.CreateFontAsset(sourceFont, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
                if (candidate == null || !candidate.TryAddCharacters(codes, out uint[] missing) || codes.Any(c => !candidate.HasCharacter((int)c)))
                    throw new InvalidOperationException("TMP could not render every generated glyph; the existing font has not been changed.");
                candidate.atlasPopulationMode = AtlasPopulationMode.Static;

                // Snapshot only files this operation owns; unrelated dirty assets are never saved.
                foreach (string path in Directory.Exists(Root) ? Directory.GetFiles(Root, "*", SearchOption.AllDirectories) : Array.Empty<string>())
                {
                    if (path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)) continue;
                    string assetPath = path.Replace('\\', '/');
                    // Font previewing dirties Unity's transient TTF glyph cache; only
                    // serialized Unity assets can have user edits to preserve here.
                    if (assetPath.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) &&
                        AssetDatabase.LoadAllAssetsAtPath(assetPath).Any(asset => asset != null && EditorUtility.IsDirty(asset)))
                        throw new InvalidOperationException("Save pending changes to " + assetPath + " before importing icons.");
                    before[assetPath] = File.ReadAllBytes(path);
                }
                foreach (var pack in packs)
                    if (pack.TmpFontAsset != null)
                    {
                        string path = AssetDatabase.GetAssetPath(pack.TmpFontAsset);
                        if (EditorUtility.IsDirty(pack.TmpFontAsset))
                            throw new InvalidOperationException("Save pending changes to " + path + " before importing icons.");
                        if (File.Exists(path)) before[path] = File.ReadAllBytes(path);
                    }
                committing = true;
                EnsureFolder(Root);
                EnsureFolder(Root + "/Sources");
                foreach (string file in Directory.GetFiles(output, "*", SearchOption.AllDirectories))
                {
                    string target = Root + file.Substring(output.Length).Replace('\\', '/');
                    TrackNew(target, before, created);
                    File.Copy(file, target, true);
                    AssetDatabase.ImportAsset(target, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                }
                var liveFont = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
                SetSource(candidate);
                var liveTmp = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(TmpPath);
                if (liveTmp == null)
                {
                    TrackNew(TmpPath, before, created);
                    liveTmp = ScriptableObject.CreateInstance<TMP_FontAsset>();
                    AssetDatabase.CreateAsset(liveTmp, TmpPath);
                }
                ReplaceAtlas(liveTmp, candidate);
                var iconPack = AssetDatabase.LoadAssetAtPath<IconFont>(PackPath);
                if (iconPack == null)
                {
                    TrackNew(PackPath, before, created);
                    iconPack = ScriptableObject.CreateInstance<IconFont>();
                    AssetDatabase.CreateAsset(iconPack, PackPath);
                }
                var serialized = new SerializedObject(iconPack);
                serialized.FindProperty("font").objectReferenceValue = liveFont;
                serialized.FindProperty("codepoints").objectReferenceValue = AssetDatabase.LoadAssetAtPath<TextAsset>(CodepointsPath);
                serialized.FindProperty("tmpFontAsset").objectReferenceValue = liveTmp;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(iconPack);
                AssetDatabase.SaveAssetIfDirty(iconPack);

                foreach (var pack in packs)
                {
                    var tmp = pack.TmpFontAsset;
                    if (tmp == null || tmp == liveTmp) continue;
                    var fallbacks = tmp.fallbackFontAssetTable ?? new List<TMP_FontAsset>();
                    if (fallbacks.Contains(liveTmp)) continue;
                    fallbacks.Add(liveTmp);
                    tmp.fallbackFontAssetTable = fallbacks;
                    EditorUtility.SetDirty(tmp);
                    AssetDatabase.SaveAssetIfDirty(tmp);
                }
                foreach (var text in Resources.FindObjectsOfTypeAll<TMP_Text>())
                    if (!EditorUtility.IsPersistent(text)) text.SetAllDirty();
                Debug.Log($"Custom icon font rebuilt: {codes.Length} glyph(s), {liveTmp.atlasTextures.Length} atlas texture(s). Sources: {Root}");
                return iconPack;
            }
            catch
            {
                if (committing)
                {
                    foreach (string path in created.OrderByDescending(p => p.Length)) AssetDatabase.DeleteAsset(path);
                    foreach (var entry in before) File.WriteAllBytes(entry.Key, entry.Value);
                    foreach (string path in before.Keys) AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                }
                throw;
            }
            finally
            {
                if (candidate != null)
                {
                    foreach (var texture in candidate.atlasTextures) if (texture != null && !EditorUtility.IsPersistent(texture)) Object.DestroyImmediate(texture);
                    if (candidate.material != null && !EditorUtility.IsPersistent(candidate.material)) Object.DestroyImmediate(candidate.material);
                    Object.DestroyImmediate(candidate);
                }
                AssetDatabase.DeleteAsset(stage);
                if (Directory.Exists(work)) Directory.Delete(work, true);
                AssetDatabase.AllowAutoRefresh();
                EditorApplication.UnlockReloadAssemblies();
                EditorUtility.ClearProgressBar();
            }
        }

        private static void TrackNew(string path, Dictionary<string, byte[]> before, HashSet<string> created)
        {
            if (!before.ContainsKey(path)) created.Add(path);
        }

        private static void SetSource(TMP_FontAsset tmp)
        {
            var serialized = new SerializedObject(tmp);
            serialized.FindProperty("m_SourceFontFileGUID").stringValue = AssetDatabase.AssetPathToGUID(FontPath);
            serialized.FindProperty("m_SourceFontFile").objectReferenceValue = null;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ReplaceAtlas(TMP_FontAsset target, TMP_FontAsset source)
        {
            var previousTextures = target.atlasTextures ?? Array.Empty<Texture2D>();
            var textures = new Texture2D[source.atlasTextures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                if (i < previousTextures.Length && previousTextures[i] != null)
                {
                    textures[i] = previousTextures[i];
                    EditorUtility.CopySerialized(source.atlasTextures[i], textures[i]);
                }
                else
                {
                    textures[i] = Object.Instantiate(source.atlasTextures[i]);
                    AssetDatabase.AddObjectToAsset(textures[i], target);
                }
                textures[i].name = "Custom Icons Atlas " + i;
                EditorUtility.SetDirty(textures[i]);
            }
            var material = target.material;
            if (material == null)
            {
                material = Object.Instantiate(source.material);
                material.name = "Custom Icons Material";
                AssetDatabase.AddObjectToAsset(material, target);
            }
            material.mainTexture = textures[0];
            EditorUtility.SetDirty(material);
            var fallbacks = target.fallbackFontAssetTable;
            EditorUtility.CopySerialized(source, target);
            target.name = "Custom Icons SDF";
            target.atlasTextures = textures;
            target.material = material;
            target.fallbackFontAssetTable = fallbacks ?? new List<TMP_FontAsset>();
            target.ReadFontAssetDefinition();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
        }

        private static IEnumerable<uint> ParseCodes(string text)
        {
            foreach (string line in (text ?? "").Split('\n'))
            {
                var fields = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length >= 2 && uint.TryParse(fields[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint code)) yield return code;
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        private static string FindTools()
        {
            string script = AssetDatabase.FindAssets("IconSvgImporter t:MonoScript").Select(AssetDatabase.GUIDToAssetPath)
                .First(p => Path.GetFileName(p) == "IconSvgImporter.cs");
            return Path.GetFullPath(Path.GetDirectoryName(script) + "/Tools");
        }

        private static string EnsurePython(string tools)
        {
            bool windows = Application.platform == RuntimePlatform.WindowsEditor;
            string env = Path.GetFullPath("Library/IconFontTools/venv");
            string python = env + (windows ? "/Scripts/python.exe" : "/bin/python");
            string requirements = File.ReadAllText(tools + "/requirements.txt");
            string stamp = env + "/icon-requirements.txt";
            if (!File.Exists(python))
            {
                string systemPython = System.Environment.GetEnvironmentVariable("ICON_FONT_PYTHON");
                if (string.IsNullOrEmpty(systemPython))
                    systemPython = windows ? "python" : File.Exists("/opt/homebrew/bin/python3") ? "/opt/homebrew/bin/python3" : "python3";
                Run(systemPython, "-m", "venv", env);
            }
            if (!File.Exists(stamp) || File.ReadAllText(stamp) != requirements)
            {
                Run(python, "-m", "pip", "install", "--disable-pip-version-check", "-r", tools + "/requirements.txt");
                File.WriteAllText(stamp, requirements);
            }
            return python;
        }

        private static void Run(string executable, params string[] args)
        {
            var info = new ProcessStartInfo(executable, string.Join(" ", args.Select(Quote)))
            {
                UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true,
                CreateNoWindow = true, WorkingDirectory = Directory.GetCurrentDirectory()
            };
            using (var process = Process.Start(info))
            {
                if (process == null) throw new InvalidOperationException("Could not start Python. Install Python 3 or set ICON_FONT_PYTHON.");
                var stdout = process.StandardOutput.ReadToEndAsync();
                var stderr = process.StandardError.ReadToEndAsync();
                if (!process.WaitForExit(60000))
                {
                    process.Kill();
                    throw new TimeoutException("Icon font generation timed out. No live assets were changed.");
                }
                if (process.ExitCode != 0)
                    throw new InvalidOperationException("Icon font generator failed:\n" + stderr.Result + stdout.Result);
            }
        }

        private static string Quote(string value)
        {
            if (value == null || value.IndexOfAny(new[] { '\r', '\n', '\0' }) >= 0)
                throw new ArgumentException("Invalid generator argument");
            // ProcessStartInfo arguments use command-line quoting, never a shell.
            string escaped = Regex.Replace(value, "(\\\\*)\"", "$1$1\\\"");
            escaped = Regex.Replace(escaped, "(\\\\+)$", "$1$1");
            return "\"" + escaped + "\"";
        }
    }
}
