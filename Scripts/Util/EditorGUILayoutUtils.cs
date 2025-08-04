using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace ElasticSea.Framework.Util
{
    public static class EditorGUILayoutUtils
    {
        public static void Line(int height = 1, Color? color = null)
        {
            var rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, color ?? new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}
#endif