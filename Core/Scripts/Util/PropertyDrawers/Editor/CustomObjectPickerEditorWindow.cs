using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Util.PropertyDrawers
{
    public class CustomObjectPickerEditorWindow : EditorWindow
    {
        public static void GetWindow(System.Type type, CustomObjectPickerAttribute attr, System.Action<Object> callback)
        {
            GetWindow<CustomObjectPickerEditorWindow>(false, "Select Object", true);

            _callback = callback;
            FilterAssets(type, attr);
        }

        private static IEnumerable<Object> _allMatchingObjects;
        private static System.Action<Object> _callback;
        private Vector2 _scrollPos = Vector2.zero;


        protected void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                if (GUILayout.Button("None"))
                    _callback.Invoke(null);

                foreach (var obj in _allMatchingObjects)
                {
                    if (GUILayout.Button(obj.name))
                        _callback.Invoke(obj);
                }
            }
            EditorGUILayout.EndScrollView();
        }


        private static void FilterAssets(System.Type type, CustomObjectPickerAttribute attr)
        {
            _allMatchingObjects = Resources.FindObjectsOfTypeAll(type);

            if (typeof(GameObject).IsAssignableFrom(type))
            {
                if (attr.resultObjectType != CustomObjectPickerAttribute.ResultObjectType.SceneOrAsset)
                {
                    if (attr.resultObjectType == CustomObjectPickerAttribute.ResultObjectType.Scene)
                        _allMatchingObjects = _allMatchingObjects.Where(t => PrefabUtility.GetPrefabType(t as GameObject) != PrefabType.Prefab);

                    if (attr.resultObjectType == CustomObjectPickerAttribute.ResultObjectType.Asset)
                        _allMatchingObjects = _allMatchingObjects.Where(t => PrefabUtility.GetPrefabType(t as GameObject) == PrefabType.Prefab);
                }

                // if we're dealing with GameObject references, then we'll restrict outrselves to any
                // GameObject with components attached that possess all type limitations collectively
                foreach (var restrictionType in attr.typeRestrictions)
                    _allMatchingObjects = _allMatchingObjects.Where(t => (t as GameObject).GetComponent(restrictionType) != null).ToList();
            }
            else if (typeof(Component).IsAssignableFrom(type))
            {
                if (attr.resultObjectType != CustomObjectPickerAttribute.ResultObjectType.SceneOrAsset)
                {
                    if (attr.resultObjectType == CustomObjectPickerAttribute.ResultObjectType.Scene)
                        _allMatchingObjects = _allMatchingObjects.Where(t => PrefabUtility.GetPrefabType((t as Component).gameObject) != PrefabType.Prefab);

                    if (attr.resultObjectType == CustomObjectPickerAttribute.ResultObjectType.Asset)
                        _allMatchingObjects = _allMatchingObjects.Where(t => PrefabUtility.GetPrefabType((t as Component).gameObject) == PrefabType.Prefab);
                }

                // if we're dealing with components, then we limit ourselves to components that derive
                // or implement all restriction type
                foreach (var restrictionType in attr.typeRestrictions)
                    _allMatchingObjects = _allMatchingObjects.Where(t => restrictionType.IsAssignableFrom(t.GetType()));
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                // ScriptableObjects are assets only, so we can skip the asset/scene object check
                foreach (var restrictionType in attr.typeRestrictions)
                    _allMatchingObjects = _allMatchingObjects.Where(t => restrictionType.IsAssignableFrom(t.GetType()));
            }
        }
    }
}