using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Misc.Editor
{
    [CustomEditor(typeof(BoundsTest))]
    public class BoundsTestEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var boundsTest = (BoundsTest)target;

            if (GUILayout.Button("SpawnPoints"))
            {
                boundsTest.SpawnPoints();
            }
            
            GUILayout.Label(boundsTest.Elapsed.ToString());
        }
    }
}