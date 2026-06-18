using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    public class SceneList : EditorWindow
    {
        private SceneListData data;
        private Vector2 scroll;
        private string[] lastSceneGuids = Array.Empty<string>();

        [MenuItem("Window/Scene List")]
        public static void ShowWindow() => GetWindow(typeof(SceneList), false, "Scenes");

        private void OnEnable()
        {
            data = SceneListData.GetOrCreate();
        }

        private void Update()
        {
            // Repaint when scenes are added/removed from Build Settings externally.
            var current = EditorBuildSettings.scenes.Select(s => s.guid.ToString()).ToArray();
            if (current.SequenceEqual(lastSceneGuids) == false)
            {
                lastSceneGuids = current;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (data == null) data = SceneListData.GetOrCreate();

            var buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length == 0)
            {
                EditorGUILayout.HelpBox("No scenes in Build Settings.", MessageType.Info);
                return;
            }

            var sceneByGuid = new Dictionary<string, EditorBuildSettingsScene>();
            foreach (var scene in buildScenes)
            {
                sceneByGuid[scene.guid.ToString()] = scene;
            }

            // Add entries for new scenes, drop entries for deleted ones.
            var changed = SyncEntries(sceneByGuid.Keys);

            // Only scenes still in Build Settings, favorites floated to the top.
            // OrderByDescending is a stable sort, so the saved relative order is
            // preserved within each group.
            var displayed = data.entries
                .Where(e => sceneByGuid.ContainsKey(e.guid))
                .OrderByDescending(e => e.favorite)
                .ToList();

            var favoriteCount = displayed.Count(e => e.favorite);
            var anyFavorites = favoriteCount > 0;

            Action deferred = null;

            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (var i = 0; i < displayed.Count; i++)
            {
                if (anyFavorites && i == 0)
                {
                    EditorGUILayout.LabelField("★ Favorites", EditorStyles.boldLabel);
                }
                if (anyFavorites && i == favoriteCount)
                {
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Scenes", EditorStyles.boldLabel);
                }

                var entry = displayed[i];
                var action = DrawRow(entry, sceneByGuid[entry.guid], displayed, i, favoriteCount);
                if (action != null) deferred = action;
            }

            EditorGUILayout.EndScrollView();

            // Apply mutations after the layout pass to avoid changing the row
            // count mid-frame; the resulting Repaint redraws with fresh state.
            if (deferred != null)
            {
                deferred();
                changed = true;
            }

            if (changed)
            {
                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssetIfDirty(data);
                Repaint();
            }
        }

        private bool SyncEntries(ICollection<string> buildGuids)
        {
            var changed = false;

            // Drop entries whose scene asset no longer exists. Scenes merely
            // removed from Build Settings are kept so their favorite/order
            // returns if they are re-added. Anything still in Build Settings is
            // left alone (a missing-asset build scene must not be re-added every
            // frame, which would thrash saves).
            for (var i = data.entries.Count - 1; i >= 0; i--)
            {
                var guid = data.entries[i].guid;
                if (buildGuids.Contains(guid) == false &&
                    string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    data.entries.RemoveAt(i);
                    changed = true;
                }
            }

            var known = new HashSet<string>(data.entries.Select(e => e.guid));
            foreach (var guid in buildGuids)
            {
                if (known.Add(guid))
                {
                    data.entries.Add(new SceneListData.Entry { guid = guid, favorite = false });
                    changed = true;
                }
            }

            return changed;
        }

        private Action DrawRow(SceneListData.Entry entry, EditorBuildSettingsScene scene, List<SceneListData.Entry> displayed, int index, int favoriteCount)
        {
            Action action = null;

            EditorGUILayout.BeginHorizontal();

            var prevColor = GUI.backgroundColor;

            // Favorite toggle.
            GUI.backgroundColor = entry.favorite ? new Color(1f, 0.85f, 0.3f) : prevColor;
            if (GUILayout.Button(entry.favorite ? "★" : "☆", GUILayout.Width(26)))
            {
                action = () => ToggleFavorite(entry);
            }
            GUI.backgroundColor = prevColor;

            // Open scene, tinted by whether it is enabled in the build.
            var name = Path.GetFileNameWithoutExtension(scene.path);
            GUI.backgroundColor = scene.enabled ? new Color(0.9f, 0.9f, 0.9f) : new Color(0.4f, 0.4f, 0.4f);
            if (GUILayout.Button(name))
            {
                var path = scene.path;
                action = () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(path);
                    }
                };
            }
            GUI.backgroundColor = prevColor;

            // Reorder within the same group (favorites or non-favorites).
            var groupStart = entry.favorite ? 0 : favoriteCount;
            var groupEnd = entry.favorite ? favoriteCount : displayed.Count;

            using (new EditorGUI.DisabledScope(index <= groupStart))
            {
                if (GUILayout.Button("▲", GUILayout.Width(26)))
                {
                    var other = displayed[index - 1];
                    action = () => SwapInData(entry, other);
                }
            }
            using (new EditorGUI.DisabledScope(index >= groupEnd - 1))
            {
                if (GUILayout.Button("▼", GUILayout.Width(26)))
                {
                    var other = displayed[index + 1];
                    action = () => SwapInData(entry, other);
                }
            }

            EditorGUILayout.EndHorizontal();

            return action;
        }

        private void ToggleFavorite(SceneListData.Entry entry)
        {
            // Move the entry to the boundary between the two groups so it lands
            // at the bottom of Favorites (when favorited) or the top of Scenes
            // (when un-favorited), rather than at an arbitrary saved position.
            entry.favorite = !entry.favorite;
            data.entries.Remove(entry);
            var boundary = data.entries.Count(e => e.favorite);
            data.entries.Insert(boundary, entry);
        }

        private void SwapInData(SceneListData.Entry a, SceneListData.Entry b)
        {
            var ia = data.entries.IndexOf(a);
            var ib = data.entries.IndexOf(b);
            if (ia < 0 || ib < 0) return;
            (data.entries[ia], data.entries[ib]) = (data.entries[ib], data.entries[ia]);
        }
    }
}
