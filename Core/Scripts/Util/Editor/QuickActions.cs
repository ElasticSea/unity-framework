using System.IO;
using Core.Extensions;
using Core.Util;
using UnityEditor;
using UnityEngine;

namespace Core.Util
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
        }
    }
}