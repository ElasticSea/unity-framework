using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    [CustomEditor(typeof(InspectorCategory))]
    public class InspectorCategoryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var imageCategory = (InspectorCategory)target;
            var rect = EditorGUILayout.GetControlRect(false, 0f);
            rect.height = EditorGUIUtility.singleLineHeight * 1.4f;
            rect.y -= rect.height;
            rect.x = 0;
            rect.xMax += 30;
            rect.yMax = 15;

            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 1f));

            var black = new GUIStyle(EditorStyles.label);
            black.normal.textColor = Color.white;
            black.fontSize = 18;

            var textRect = new Rect(rect);
            textRect.x += 10;
            imageCategory.Category = EditorGUI.TextField(textRect, imageCategory.Category, black);
        }
    }
}