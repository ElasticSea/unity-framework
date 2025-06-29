using DG.Tweening;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Ui.Icons;
using ElasticSea.Framework.Ui.Interactions;
using ElasticSea.Framework.Util;
using Interactions;
using Unity.PolySpatial;
using UnityEngine;

namespace ElasticSea.Framework.Ui.VisionOs
{
    public class VisionFlatMeshIconFactory : MonoBehaviour
    {
        [SerializeField] private FlatMeshIconFactory factory;

        [SerializeField] private Mesh circleMesh;
        [SerializeField] private Material circleMaterial;

        public FlatMeshIcon[] Build(FlatMeshIconData[] iconData)
        {
            var icons = factory.Build(iconData, circleMesh, circleMaterial);

            foreach (var icon in icons)
            {
                var anim = new HideShowInternalAnim(DOTween.To(t => icon.Material.SetFloat("_Pressed", t), 0, 1, 0.15f));
                icon.FocusTransition = anim;

                var interactable = new SimpleInteractable()
                {
                    PressCallback = @event =>
                    {
                        icon.Focus();
                    },
                    CancelCallback = @event =>
                    {
                        icon.Unfocus();
                    },
                    ReleaseCallback = @event =>
                    {
                        icon.Unfocus();
                    }
                };
                icon.Interactable = interactable;
                
                var interactableComponent =  icon.gameObject.GetOrAddComponent<IteractableComponent>();
                interactableComponent.Interactable = interactable;
                
#if UNITY_VISIONOS
                var visionOSHoverEffect = icon.gameObject.AddComponent<VisionOSHoverEffect>();
                visionOSHoverEffect.Type = VisionOSHoverEffect.EffectType.Shader;
                visionOSHoverEffect.FadeInDuration = 0.4f;; 
                visionOSHoverEffect.FadeOutDuration = 0.4f;; 
                
                icon.gameObject.layer = LayerMask.NameToLayer("VisionOsCollisionLayer");
                interactableComponent.CancelByMove = true;
#endif
                
            }

            return icons;
        }
    
        private class HideShowInternalAnim : IHideShowAnim
        {
            private readonly Tween tween;

            public HideShowInternalAnim(Tween tween)
            {
                this.tween = tween.SetAutoKill(false).Pause();
            }
        
            public void Show(bool animate = true)
            {
                tween.PlayForward();
                if (animate == false) tween.Complete();
            }

            public void Hide(bool animate = true)
            {
                tween.PlayBackwards();
                if (animate == false) tween.Goto(0, true);
            }
        }
    }
}