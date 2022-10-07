using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Util.Editor
{
    [CustomEditor(typeof(CanvasScreenshotUtility))]
    public class CanvasScreenshotUtilityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Render"))
            {
                (target as CanvasScreenshotUtility).Render();
            }
        }
    }
}