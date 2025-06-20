using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEditor;
using UnityEngine;
using EditorUtils = ElasticSea.Framework.Scripts.Util.Editor.EditorUtils;

namespace ElasticSea.Framework.Scripts.Util.Icons
{
    public class IconExplorer : EditorWindow
    {
        private IconFont pack;
        private string filterName;
        private int limit = 10;
        private bool dirty = true;
        private IEnumerable<(string name, int code)> configIcons;
        private Vector2 scrollPos;

        [MenuItem("Window/Icon Explorer")]
        public static IconExplorer Show()
        {
            return GetWindow<IconExplorer>("Icon Explorer");
        }

        private void OnGUI()
        {
            var prevPack = pack;
            pack = (IconFont) EditorGUILayout.ObjectField(pack, typeof(IconFont), true);
            
            GUILayout.BeginHorizontal();
            foreach (var font in EditorUtils.FindAssetsByType<IconFont>())
            {
                if (GUILayout.Button(font.name))
                {
                    pack = font;
                }
            }
            GUILayout.EndHorizontal();
          
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
                var codepoints = new HashSet<int>();
                foreach (var c in  filterName.Split(",").Select(s => ParseNoException(s, NumberStyles.HexNumber)).Where(c => c != 0))
                {
                    codepoints.Add(c);
                }
                foreach (var c in  filterName.Split(",").Where(c => c.Length > 0))
                {
                    codepoints.Add(c[0]);
                }
                
                configIcons = GenerateConfig(pack)
                    .Where(pair => filterName.IsNullOrEmpty() || pair.name.Contains(filterName) || codepoints.Contains(pair.code))
                    .Take(limit)
                    .ToList();
            }

            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var (name, code) in configIcons.Select(x => (x.name, x.code)))
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

        private int ParseNoException(string text, NumberStyles numberStyles)
        {
            try
            {
                return int.Parse(text, numberStyles);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        private IEnumerable<(string name, int code)> GenerateConfig(IconFont iconPack)
        {
            return iconPack.CodePoints
                .Replace('\r', '\n')
                .Split('\n')
                .Where(s => s.IsNullOrEmpty() == false)
                .Select(s =>
                {
                    var split = s.Split(' ');
                    var name = split[0];
                    var code = int.Parse(split[1], System.Globalization.NumberStyles.HexNumber);
                    return (name, code);
                });
        }
    }
}
