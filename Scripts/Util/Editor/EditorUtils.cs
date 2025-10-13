using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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
        
        public static VisualElement BuildInspectorPropertiesVisualElement(SerializedObject obj)
        {
            VisualElement container = new VisualElement {name = obj.targetObject.name};
          
            SerializedProperty iterator = obj.GetIterator();
            Type targetType = obj.targetObject.GetType();
            List<MemberInfo> members = new List<MemberInfo>(targetType.GetMembers());

            if (!iterator.NextVisible(true)) return container;
            do
            {
                if (iterator.propertyPath == "m_Script" && obj.targetObject != null)
                {
                    continue;
                }
                PropertyField propertyField = new PropertyField(iterator.Copy())
                {
                    name = "PropertyField:" + iterator.propertyPath,
                };
                propertyField.Bind(iterator.serializedObject);

                MemberInfo member = members.Find(x => x.Name == propertyField.bindingPath);
                if (member != null)
                {
                    IEnumerable<Attribute> headers = member.GetCustomAttributes(typeof(HeaderAttribute));
                    IEnumerable<Attribute> spaces = member.GetCustomAttributes(typeof(SpaceAttribute));
                    foreach (Attribute x in headers)
                    {
                        HeaderAttribute actual = (HeaderAttribute) x;
                        Label header = new Label { text = actual.header};
                        header.style.unityFontStyleAndWeight = FontStyle.Bold;
                        container.Add(new Label { text = " ", name = "Header Spacer"});
                        container.Add(header);
                    }
                    foreach (Attribute unused in spaces)
                    {
                        container.Add(new Label { text = " " });
                    }
                }

                container.Add(propertyField);
            }
            while (iterator.NextVisible(false));

            return container;
        }
        
        /// <summary>
        /// Creates or updates a prefab at <paramref name="prefabPath"/> from <paramref name="source"/>.
        /// Uses EditPrefabContentsScope to preserve local fileIDs and references.
        /// </summary>
        public static GameObject CreateOrUpdatePrefab(GameObject source, string prefabPath)
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));

            // Create directories if missing
            Directory.CreateDirectory(Path.GetDirectoryName(prefabPath)!);

            // Case 1: Prefab does not exist → create it once
            if (!File.Exists(prefabPath))
            {
                var created = PrefabUtility.SaveAsPrefabAsset(source, prefabPath);
                return created;
            }

            // Case 2: Prefab exists → open for editing (fileIDs preserved)
            using (var editScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
            {
                var prefabRoot = editScope.prefabContentsRoot;

                // Copy main GameObject data
                prefabRoot.name = source.name;
                prefabRoot.transform.localPosition = source.transform.localPosition;
                prefabRoot.transform.localRotation = source.transform.localRotation;
                prefabRoot.transform.localScale    = source.transform.localScale;

                // Copy serialized component data
                EditorUtility.CopySerialized(source, prefabRoot);

                // Synchronize hierarchy (by name, non-destructive)
                SyncChildren(source.transform, prefabRoot.transform);
            }

            // The scope auto-saves and unloads here
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        private static void SyncChildren(Transform src, Transform dst)
        {
            // Ensure all source children exist in destination and copy them
            for (int i = 0; i < src.childCount; i++)
            {
                var srcChild = src.GetChild(i);
                var dstChild = dst.Find(srcChild.name);

                if (!dstChild)
                {
                    var newChild = new GameObject(srcChild.name);
                    newChild.transform.SetParent(dst, false);
                    dstChild = newChild.transform;
                }

                EditorUtility.CopySerialized(srcChild.gameObject, dstChild.gameObject);
                SyncChildren(srcChild, dstChild);
            }

            // Remove any extra children not in source
            for (int i = dst.childCount - 1; i >= 0; i--)
            {
                var dstChild = dst.GetChild(i);
                if (!src.Find(dstChild.name))
                    Object.DestroyImmediate(dstChild.gameObject);
            }
        }
    }
}