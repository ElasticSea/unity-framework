using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    public class SceneListData : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string guid;
            public bool favorite;
        }

        public List<Entry> entries = new List<Entry>();

        private const string DefaultPath = "Assets/Misc/SceneListData.asset";

        public static SceneListData GetOrCreate()
        {
            var found = AssetDatabase.FindAssets($"t:{nameof(SceneListData)}");
            if (found.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(found[0]);
                return AssetDatabase.LoadAssetAtPath<SceneListData>(path);
            }

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(DefaultPath));
            var asset = CreateInstance<SceneListData>();
            AssetDatabase.CreateAsset(asset, DefaultPath);
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
