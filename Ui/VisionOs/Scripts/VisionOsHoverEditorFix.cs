using DG.Tweening;
using ElasticSea.Framework.Ui.Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace ElasticSea.Framework.Ui.VisionOs
{
    public class VisionOsHoverEditorFix
    {
        public static void Setup(IteractableComponent iteractableComponent)
        {
#if UNITY_VISIONOS && UNITY_EDITOR
            var original = iteractableComponent.Interactable;

            void ApplyHover(float t, GameObject gameObject)
            {
                var renderers = gameObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    foreach (var material in materials)
                    {
                        material.SetFloat("_EditorHover", t);
                    }
                }
                
                var graphics = gameObject.GetComponentsInChildren<Graphic>();
                foreach (var graphic in graphics)
                {
                    graphic.material.SetFloat("_EditorHover", t);
                }
                
                var tmpTexts = gameObject.GetComponentsInChildren<TMP_Text>();
                foreach (var tmpText in tmpTexts)
                {
                    tmpText.fontMaterial.SetFloat("_EditorHover", t);
                }
            }
            
            var hoverAnim = new HideShowAnimation(DOTween.To(value => ApplyHover(value, iteractableComponent.gameObject), 0, 1, 0.15f));
            
            iteractableComponent.Interactable = new SimpleInteractable()
            {
                HighlightCallback = _ => hoverAnim.Show(),
                UnhighlightCallback = _ => hoverAnim.Hide(),
                PressCallback = @event => original.Press(@event),
                ReleaseCallback = @event => original.Release(@event),
                CancelCallback = @event => original.Cancel(@event),
                MoveCallback = @event => original.Move(@event)
            };
#endif
        }
    }
}