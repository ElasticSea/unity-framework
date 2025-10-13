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
    }
}