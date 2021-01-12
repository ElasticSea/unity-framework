using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.Util.PropertyDrawers
{
    using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(CustomObjectPickerAttribute), true)]
    public class CustomObjectPickerPropertyDrawer : PropertyDrawer
    {
        private static Type GameObjectType = typeof(GameObject);
        private static Type ScriptableObjectType = typeof(ScriptableObject);
        private static Type ComponentType = typeof(Component);


        private static Type _unityObjectPickerType = null;

        private static Type unityObjectPickerType
        {
            get
            {
                if (_unityObjectPickerType == null)
                    _unityObjectPickerType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("ObjectSelector");

                return _unityObjectPickerType;
            }
        }

        private static PropertyInfo _unityObjectPickerIsVisibleProperty = null;

        private static PropertyInfo unityObjectPickerIsVisibleProperty
        {
            get
            {
                if (_unityObjectPickerIsVisibleProperty == null)
                    _unityObjectPickerIsVisibleProperty =
                        unityObjectPickerType.GetProperty("isVisible", BindingFlags.Public | BindingFlags.Static);

                return _unityObjectPickerIsVisibleProperty;
            }
        }

        private static bool isUnityObjectPickerVisible
        {
            get { return (bool) unityObjectPickerIsVisibleProperty.GetValue(null, null); }
        }


        private GUIStyle _infoBoxStyle = null;

        private GUIStyle infoBoxStyle
        {
            get
            {
                if (_infoBoxStyle == null)
                    // _infoBoxStyle = new GUIStyle(Utilities.UnityEditorStyleUtil.ErrorLabelStyle(Utilities.UnityEditorStyleUtil.UnitySkin.Light));
                    _infoBoxStyle = new GUIStyle();

                return _infoBoxStyle;
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!typeof(Object).IsAssignableFrom(fieldInfo.FieldType) || property.isArray)
            {
                EditorGUILayout.LabelField("Field type not supported for CustomObjectPicker.", infoBoxStyle);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            RenderObjectPicker(position, label.text, property, fieldInfo.FieldType, attribute as CustomObjectPickerAttribute);
            EditorGUI.EndProperty();
        }


        private static void RenderObjectPicker(Rect rect, string fieldName, SerializedProperty prop, Type reqObjType,
            CustomObjectPickerAttribute attr)
        {
            GUIContent content = GetContentFromObject(prop.objectReferenceValue, reqObjType);
            ObjectField(rect, rect, new GUIContent(fieldName), 1, prop.objectReferenceValue, reqObjType, attr, (val) =>
            {
                prop.objectReferenceValue = val;

                prop.serializedObject.ApplyModifiedProperties();
                prop.serializedObject.Update();
            });
        }

        private static void ObjectField(Rect position, Rect dropRect, GUIContent fieldName, int id, Object obj, Type reqObjType,
            CustomObjectPickerAttribute attr, Action<Object> callback)
        {
            Event current = Event.current;
            EventType eventType = current.type;

            if (!GUI.enabled && Event.current.rawType == EventType.MouseDown)
                eventType = Event.current.rawType;

            if (fieldName != null && string.IsNullOrEmpty(fieldName.text) == false)
            {
                var labelPos = position;
                labelPos.width = EditorGUIUtility.labelWidth;
                EditorGUI.PrefixLabel(labelPos, fieldName);

                position.x += labelPos.width;
                position.width -= labelPos.width;
            }

            EditorGUI.LabelField(position, obj ? $"{obj.name} [{obj.GetType().Name}]" : "None", EditorStyles.objectField);
            position.width -= 32;

            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp)
            {
                if (position.Contains(Event.current.mousePosition) && obj != null)
                {
                    Selection.objects = new[] {obj};
                    EditorGUIUtility.PingObject(obj);
                    Event.current.Use();
                    GUIUtility.ExitGUI();
                }

                position.x += position.width;
                position.width = 32;

                if (position.Contains(Event.current.mousePosition))
                {
                    CustomObjectPickerEditorWindow.GetWindow(reqObjType, attr, (val) =>
                    {
                        if (callback != null)
                            callback(val);

                        GUI.changed = true;
                    });
                }
            }

            EventType eventType2 = eventType;
            switch (eventType2)
            {
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                case EventType.Layout:
                case EventType.Ignore:
                case EventType.Used:
                case EventType.ValidateCommand:
                case EventType.Repaint:
                case EventType.ExecuteCommand:
                    break;
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropRect.Contains(Event.current.mousePosition) && GUI.enabled)
                    {
                        Object[] objectReferences = DragAndDrop.objectReferences;
                        var obj2 = objectReferences.FirstOrDefault();

                        if (obj2 != null)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                            if (eventType == EventType.DragPerform && ValidateObject(obj2, reqObjType, attr, true))
                            {
                                obj = obj2;
                                GUI.changed = true;

                                Type objType = obj.GetType();
                                if (GameObjectType.IsAssignableFrom(objType))
                                {
                                    var objAsGameObject = obj as GameObject;

                                    if (GameObjectType.IsAssignableFrom(reqObjType))
                                        callback(obj);
                                    else if (ComponentType.IsAssignableFrom(reqObjType))
                                    {
                                        Object[] objs = (obj as GameObject).GetComponents(reqObjType);
                                        foreach (var @object in objs)
                                        {
                                            bool success = true;
                                            foreach (var restriction in attr.typeRestrictions)
                                            {
                                                if (!restriction.IsAssignableFrom(@object.GetType()))
                                                    success = false;
                                            }

                                            if (success)
                                                callback(@object);
                                        }
                                    }
                                }
                                else
                                    callback(obj);

                                DragAndDrop.AcceptDrag();
                                DragAndDrop.activeControlID = 0;
                            }
                            else
                                DragAndDrop.activeControlID = id;

                            Event.current.Use();
                        }
                    }

                    break;
                case EventType.DragExited:
                    if (GUI.enabled)
                        HandleUtility.Repaint();
                    break;
            }
        }

        private static bool ValidateObject(Object obj, Type reqObjType, CustomObjectPickerAttribute attr,
            bool includeGOComponents = false)
        {
            if (obj == null)
                return false;

            Type objType = obj.GetType();

            if (GameObjectType.IsAssignableFrom(reqObjType))
            {
                if (GameObjectType.IsAssignableFrom(objType))
                {
                    foreach (var restriction in attr.typeRestrictions)
                    {
                        if ((obj as GameObject).GetComponent(restriction) == null)
                            return false;
                    }

                    return true;
                }
            }
            else if (ComponentType.IsAssignableFrom(reqObjType))
            {
                if (includeGOComponents && GameObjectType.IsAssignableFrom(objType))
                {
                    Object[] objs = (obj as GameObject).GetComponents(reqObjType);
                    foreach (var @object in objs)
                    {
                        bool success = true;
                        foreach (var restriction in attr.typeRestrictions)
                        {
                            if (!restriction.IsAssignableFrom(@object.GetType()))
                                success = false;
                        }

                        if (success)
                            return true;
                    }

                    return false;
                }
                else
                {
                    if (ComponentType.IsAssignableFrom(objType))
                    {
                        foreach (var restriction in attr.typeRestrictions)
                        {
                            if (!restriction.IsAssignableFrom(objType))
                                return false;
                        }

                        return true;
                    }
                }
            }
            else if (ScriptableObjectType.IsAssignableFrom(reqObjType))
            {
                if (ScriptableObjectType.IsAssignableFrom(objType))
                {
                    foreach (var restriction in attr.typeRestrictions)
                    {
                        if (!restriction.IsAssignableFrom(objType))
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        private static GUIContent GetContentFromObject(Object obj, Type type)
        {
            return new GUIContent(obj == null ? "None (" + type.Name + ")" : obj.name + " (" + type.Name + ")");
        }
    }
}