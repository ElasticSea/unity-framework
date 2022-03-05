#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.Icons
{
    public class IconExplorer : EditorWindow
    {
        private IconPack pack;
        private string filterName;
        private int limit = 10;
        private bool dirty = true;
        private IEnumerable<KeyValuePair<string, int>> configIcons;
        private Vector2 scrollPos;

        [MenuItem("Window/Icon Explorer")]
        public static IconExplorer Show()
        {
            return GetWindow<IconExplorer>("Icon Explorer");
        }

        private void OnGUI()
        {
            var prevPack = pack;
            pack = (IconPack) EditorGUILayout.ObjectField(pack, typeof(IconPack), true);
            if (pack != prevPack) dirty = true;
            
            if (pack == null) return;
            var fontStyle = new GUIStyle(GUI.skin.label) {font = pack.Font, fontSize = 32};

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Name");
            var prevFilterName = filterName;
            filterName = EditorGUILayout.TextField(filterName);
            if (filterName != prevFilterName) dirty = true;
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Limit");
            var prevLimit = limit;
            limit = int.Parse(EditorGUILayout.TextField(limit.ToString()));
            if (limit != prevLimit) dirty = true;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (configIcons == null || dirty)
            {
                configIcons = pack.Icons
                    .Where(pair => filterName.IsNullOrEmpty() || pair.Key.Contains(filterName))
                    .Take(limit)
                    .ToList();
            }

            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var (name, code) in configIcons.Select(x => (x.Key, x.Value)))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(((char) code).ToString(), fontStyle, GUILayout.MinWidth(50), GUILayout.MinHeight(50),GUILayout.MaxWidth(50), GUILayout.MaxHeight(50)))
                {
                    EditorGUIUtility.systemCopyBuffer = char.ToString((char)code);   
                }
                EditorGUILayout.BeginVertical();
                EditorGUILayout.TextField(name, GUILayout.ExpandWidth(false),GUILayout.MinHeight(25));
                EditorGUILayout.TextField("U+" + code.ToString("X4"),GUILayout.MinHeight(25));
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

            dirty = false;
        }
    }
}
#endif