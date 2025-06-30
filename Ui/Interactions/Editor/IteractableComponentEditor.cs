using UnityEditor;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Interactions.Editor
{
    [CustomEditor(typeof(IteractableComponent))]
    public class IteractableComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var inspector = target as IteractableComponent;
            var interactable = inspector.Interactable;
            if (GUILayout.Button("Highlight")) interactable.Hover(null);
            if (GUILayout.Button("Unhighlight")) interactable.UnHover(null);
            if (GUILayout.Button("Press")) interactable.Press(null);
            if (GUILayout.Button("Release")) interactable.Release(null);
            if (GUILayout.Button("Cancel")) interactable.Cancel(null);
            if (GUILayout.Button("Click"))
            {
                interactable.Press(null);
                interactable.Release(null);
            }
        }
    }
}