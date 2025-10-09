#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public static class TextAssetUtility
    {
        /// <summary>
        /// Creates or overwrites a .bytes TextAsset at the given path.
        /// </summary>
        /// <param name="bytes">Binary data to write.</param>
        /// <param name="assetPath">Unity-relative path (e.g. "Assets/Data/file.bytes").</param>
        public static TextAsset CreateOrReplaceBytesAsset(byte[] bytes, string assetPath)
        {
            if (bytes == null)
                throw new System.ArgumentNullException(nameof(bytes));

            // Ensure directory exists
            string fullDir = Path.GetDirectoryName(assetPath);
            if (!Directory.Exists(fullDir))
                Directory.CreateDirectory(fullDir);

            // Write binary file
            File.WriteAllBytes(assetPath, bytes);

            // Tell Unity to reimport / refresh
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            // Load as TextAsset
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (asset == null)
                Debug.LogWarning($"Created file at {assetPath}, but failed to load as TextAsset.");
            return asset;
        }
        
        /// <summary>
        /// Updates the contents of an existing .bytes TextAsset.
        /// Does nothing if the asset does not exist.
        /// </summary>
        /// <param name="bytes">New binary data to write.</param>
        /// <param name="assetPath">Unity-relative path (e.g. "Assets/Data/file.bytes").</param>
        /// <returns>The updated TextAsset, or null if it didn’t exist.</returns>
        public static TextAsset UpdateBytesAsset(byte[] bytes, string assetPath)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (!File.Exists(assetPath))
            {
                Debug.LogWarning($"Update skipped: no existing asset at {assetPath}");
                return null;
            }

            try
            {
                File.WriteAllBytes(assetPath, bytes);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (asset == null)
                    Debug.LogWarning($"Updated file at {assetPath}, but failed to load as TextAsset.");

                return asset;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to update {assetPath}: {ex}");
                return null;
            }
        }
    }
}
#endif