using UnityEngine;

namespace ElasticSea.Framework.Interactions
{
    public class IteractableComponent : MonoBehaviour, IInteractable
    {
        public IInteractable Interactable;
        public bool IsActive = true;
        
        // Used for pinch interactions mainly on vision os
        public bool CancelByMove = false;

        public void Hover(InteractionEvent interactionEvent) => Interactable.Hover(interactionEvent);
        public void UnHover(InteractionEvent interactionEvent) => Interactable.UnHover(interactionEvent);
        public void Press(InteractionEvent interactionEvent) => Interactable.Press(interactionEvent);
        public void Release(InteractionEvent interactionEvent) => Interactable.Release(interactionEvent);
        public void Cancel(InteractionEvent interactionEvent) => Interactable.Cancel(interactionEvent);
        public void Move(InteractionEvent interactionEvent) => Interactable.Move(interactionEvent);
    }
}