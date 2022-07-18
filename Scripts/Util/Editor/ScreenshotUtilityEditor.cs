using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    [CustomEditor(typeof(ScreenshotUtility))]
    public class ScreenshotUtilityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Render"))
            {
                (target as ScreenshotUtility).Render();
            }
        }
    }
}