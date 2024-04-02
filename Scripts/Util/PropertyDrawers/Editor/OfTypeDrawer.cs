using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ElasticSea.Framework.Util.PropertyDrawers.Editor
{
    [CustomPropertyDrawer(typeof(OfTypeAttribute))]
    public class OfTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Verify the property type is supported.
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label.text, "Use OfType with ObjectReference properties only.");
                return;
            }
            OfTypeAttribute ofType = attribute as OfTypeAttribute;
            EditorGUI.BeginProperty(position, label, property);
            // Calculate the object field position and size
            Rect objectFieldPosition = position;
            objectFieldPosition.height = EditorGUIUtility.singleLineHeight;
            // Draw the object field and get the selected object
            Object objectValue = EditorGUI.ObjectField(objectFieldPosition, label, property.objectReferenceValue, fieldInfo.FieldType, true);
            // Validate the assigned object
            if (objectValue != null && !ofType.types.Any(t => t.IsAssignableFrom(objectValue.GetType())))
            {
                property.objectReferenceValue = null;
                // Adjust the position for the help box below the object field
                Rect helpBoxPosition = position;
                helpBoxPosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                helpBoxPosition.height = EditorGUIUtility.singleLineHeight * 2; // Adjust height as needed
                EditorGUI.HelpBox(helpBoxPosition, $"This field only accepts types: {string.Join(", ", ofType.types.Select(t => t.Name))}", MessageType.Error);
            }
            else
            {
                property.objectReferenceValue = objectValue;
            }
            EditorGUI.EndProperty();
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Check if the property type is of an object reference.
            if (property.propertyType != SerializedPropertyType.ObjectReference &&
                property.propertyType != SerializedPropertyType.ExposedReference)
            {
                throw new System.ArgumentException("This attribute is not supported on properties of this property type.",
                    nameof(property.propertyType));
            }

            GetFieldInfoFromProperty(property, out var type);

            var ofType = attribute as OfTypeAttribute;

            // Set up the object field.
            var objectField = new ObjectField(property.displayName);
            objectField.AddToClassList(ObjectField.alignedFieldUssClassName);
            objectField.BindProperty(property);
            objectField.objectType = type;

            // Disable dropping if not assignable from drag and drop.
            objectField.RegisterCallback<DragUpdatedEvent>(dragUpdated =>
            {
                if (!DragAndDrop.objectReferences.Any(obj
                        => obj is GameObject gameObject ? FirstValidOrDefault(gameObject) : IsValid(obj)))
                {
                    dragUpdated.PreventDefault();
                }
            });

            // Assign the appropriate value.
            objectField.RegisterValueChangedCallback(changed =>
            {
                if (IsValid(changed.newValue))
                {
                    return;
                }
                else if (changed.newValue is GameObject gameObject ||
                         changed.newValue is Component component && (gameObject = component.gameObject))
                {
                    objectField.value = FirstValidOrDefault(gameObject);
                }
                else
                {
                    objectField.value = null;
                }
            });

            return objectField;

            // Helper methods.
            bool IsValid(Object obj) =>
                !obj || type.IsAssignableFrom(obj.GetType()) && ofType.types.All(type => type.IsAssignableFrom(obj.GetType()));

            Component FirstValidOrDefault(GameObject gameObject) => gameObject.GetComponents<Component>().FirstOrDefault(IsValid);
        }

        private void GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        {
            // INTERNAL UNITY FieldInfo UnityEditor.ScriptAttributeUtility.GetFieldInfoFromProperty(SerializedProperty property, out Type type);

            // Set up the type variables.
            var internalType = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor.CoreModule");
            var method = internalType.GetMethod("GetFieldInfoFromProperty", BindingFlags.Static | BindingFlags.NonPublic);

            Type ttype = null;
            object[] args = { property, ttype };
            method.Invoke(this, args);
            type = (Type)args[1];
        }
    }
}