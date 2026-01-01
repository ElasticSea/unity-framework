using UnityEngine;

namespace ElasticSea.Framework.Interactions
{
    // Must be placed on collider that has the vision os layer
    public class VisionOsIteractableComponent : MonoBehaviour, IInteractable
    {
        public IInteractable Interactable;
        public bool IsActive = true;
        public bool CancelByMove = false;

        public void Hover(InteractionEvent interactionEvent) => Interactable.Hover(interactionEvent);
        public void UnHover(InteractionEvent interactionEvent) => Interactable.UnHover(interactionEvent);
        public void Press(InteractionEvent interactionEvent) => Interactable.Press(interactionEvent);
        public void Release(InteractionEvent interactionEvent) => Interactable.Release(interactionEvent);
        public void Cancel(InteractionEvent interactionEvent) => Interactable.Cancel(interactionEvent);
        public void Move(InteractionEvent interactionEvent) => Interactable.Move(interactionEvent);
    }
}