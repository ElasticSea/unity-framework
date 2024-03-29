﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util.Editor
{
    public class EditorUtils
    {
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
        
        public static GameObject[] GetPrefabsAtPath(string path)
        {
            var assets = AssetDatabase.FindAssets($"t:Prefab", new[] { path });
            var foundAssets = new List<GameObject>();
 
            foreach (var guid in assets)
            {
                foundAssets.Add(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)));
            }
 
            return foundAssets.ToArray();
        }
    }
}