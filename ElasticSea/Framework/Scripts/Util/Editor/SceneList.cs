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
        private IEnumerable<EditorBuildSettingsScene> scenes = Enumerable.Empty<EditorBuildSettingsScene>();

        [MenuItem("Window/Scene List")]
        public static void ShowWindow() => GetWindow(typeof(SceneList), false, "Scenes");

        private void Update()
        {
            var newScenes = EditorBuildSettings.scenes.ToList();
            if (scenes.SequenceEqual(newScenes) == false)
            {
                scenes = EditorBuildSettings.scenes.ToList();
                Repaint();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            foreach (var scene in EditorBuildSettings.scenes)
            {
                var name = Path.GetFileNameWithoutExtension(scene.path);

                GUI.backgroundColor = scene.enabled ? new Color(0.9f, 0.9f, 0.9f) : new Color(0.4f, 0.4f, 0.4f);
                if (GUILayout.Button(name))
                {
                    EditorSceneManager.OpenScene(scene.path);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}