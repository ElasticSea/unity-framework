#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public static class ScriptableObjectUtils
    {
        /// <summary>
        /// Create or overwrite a ScriptableObject asset of type T at the given Assets/ path.
        /// Example path: "Assets/Data/MySettings.asset"
        /// </summary>
        /// <typeparam name="T">ScriptableObject type</typeparam>
        /// <param name="assetPath">Unity project path starting with "Assets/"</param>
        /// <param name="init">Optional initializer to set fields on a fresh temp instance before saving</param>
        /// <returns>The created or updated asset instance in the Project</returns>
        public static T CreateOrOverwriteAsset<T>(string assetPath, Action<T> init = null) where T : ScriptableObject
        {
            if (string.IsNullOrWhiteSpace(assetPath) || !assetPath.Replace('\\','/').StartsWith("Assets/"))
                throw new ArgumentException("assetPath must start with \"Assets/\" and be non-empty.", nameof(assetPath));

            assetPath = NormalizeAssetPath(assetPath);

            // Make sure the directory exists
            EnsureFolders(Path.GetDirectoryName(assetPath)!.Replace('\\','/'));

            // If the asset already exists, overwrite its contents
            var existing = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existing != null)
            {
                var temp = ScriptableObject.CreateInstance<T>();
                try
                {
                    init?.Invoke(temp);

#if UNITY_6000_0_OR_NEWER
                    // Unity 6+: safer for managed fields
                    EditorUtility.CopySerializedManagedFieldsOnly(temp, existing);
#else
                EditorUtility.CopySerialized(temp, existing);
#endif
                    EditorUtility.SetDirty(existing);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return existing;
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(temp);
                }
            }

            // Otherwise create a new asset
            var instance = ScriptableObject.CreateInstance<T>();
            init?.Invoke(instance);

            AssetDatabase.CreateAsset(instance, assetPath);
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return instance;
        }

        /// <summary>
        /// Non-generic overload when you only have a System.Type at runtime.
        /// </summary>
        public static ScriptableObject CreateOrOverwriteAsset(string assetPath, Type type, Action<ScriptableObject> init = null)
        {
            if (type == null || !typeof(ScriptableObject).IsAssignableFrom(type))
                throw new ArgumentException("type must derive from ScriptableObject.", nameof(type));

            assetPath = NormalizeAssetPath(assetPath);
            EnsureFolders(Path.GetDirectoryName(assetPath)!.Replace('\\','/'));

            var existing = AssetDatabase.LoadAssetAtPath(assetPath, type) as ScriptableObject;
            if (existing != null)
            {
                var temp = ScriptableObject.CreateInstance(type);
                try
                {
                    init?.Invoke(temp);
#if UNITY_6000_0_OR_NEWER
                    EditorUtility.CopySerializedManagedFieldsOnly(temp, existing);
#else
                EditorUtility.CopySerialized(temp, existing);
#endif
                    EditorUtility.SetDirty(existing);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    return existing;
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(temp);
                }
            }

            var instance = ScriptableObject.CreateInstance(type);
            init?.Invoke(instance);
            AssetDatabase.CreateAsset(instance, assetPath);
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return instance;
        }

        // --- Helpers ---

        private static string NormalizeAssetPath(string assetPath)
        {
            assetPath = assetPath.Replace('\\','/');
            if (!assetPath.StartsWith("Assets/"))
                throw new ArgumentException("Path must start with Assets/");

            // Ensure .asset extension
            if (string.IsNullOrEmpty(Path.GetExtension(assetPath)))
                assetPath += ".asset";

            return assetPath;
        }

        /// <summary>
        /// Ensures the full Assets/… folder chain exists using AssetDatabase.CreateFolder.
        /// </summary>
        private static void EnsureFolders(string directoryAssetPath)
        {
            if (string.IsNullOrEmpty(directoryAssetPath)) return;

            directoryAssetPath = directoryAssetPath.Replace('\\','/');
            if (AssetDatabase.IsValidFolder(directoryAssetPath)) return;

            var parts = directoryAssetPath.Split('/');
            if (parts.Length == 0 || parts[0] != "Assets")
                throw new ArgumentException("Directory must be under Assets/");

            string current = "Assets";
            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
