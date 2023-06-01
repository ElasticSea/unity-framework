using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    public class QuickActions : EditorWindow
    {
        [MenuItem("Window/Quick Actions")]
        public static void ShowWindow() => GetWindow(typeof(QuickActions), false, "Quick Actions");

        private void OnGUI()
        {
            if (GUILayout.Button("Show Persistent Folder"))
            {
                Utils.OpenInExplorer(Application.persistentDataPath);
            }
            
            if (GUILayout.Button("Clear Persistent Folder"))
            {
                Directory.Delete(Application.persistentDataPath, true);
            }
            
            if (GUILayout.Button("Build Folder"))
            {
                Utils.OpenInExplorer(Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "Build"));
            }

            if (GUILayout.Button("Bump Build Version"))
            {
                PlayerSettings.Android.bundleVersionCode++;
                var iosBuildNumber = PlayerSettings.iOS.buildNumber.ToNullableInt();
                if (iosBuildNumber != null)
                {
                    PlayerSettings.iOS.buildNumber = (iosBuildNumber + 1).ToString();
                }

                var macosBuildNumber = PlayerSettings.macOS.buildNumber.ToNullableInt();
                if (macosBuildNumber != null)
                {
                    PlayerSettings.macOS.buildNumber = (macosBuildNumber + 1).ToString();
                }
            }

            if (GUILayout.Button("Bump Minor Version"))
            {
                var version = new Version(Application.version);

                var main = version.Minor;
                var patch = version.Build+ 1;
                var build = 0;
                UpdateVersion(main, patch, build);
            }

            if (GUILayout.Button("Bump Major Version"))
            {
                var version = new Version(Application.version);

                var main = version.Minor + 1;
                var patch = 0;
                var build = 0;
                UpdateVersion(main, patch, build);
            }
        }

        private void UpdateVersion(int main, int patch, int build)
        {
            PlayerSettings.iOS.buildNumber = build.ToString();
            PlayerSettings.macOS.buildNumber = build.ToString();
            PlayerSettings.Android.bundleVersionCode = Convert.ToInt32($"{main}{patch}{build}");
            PlayerSettings.bundleVersion = $"0.{main}.{patch}";
        }
    }
}