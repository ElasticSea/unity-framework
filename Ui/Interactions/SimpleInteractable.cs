using System;

namespace ElasticSea.Framework.Ui.Interactions
{
    public class SimpleInteractable : IInteractable
    {
        public Action<InteractionEvent> HighlightCallback;
        public Action<InteractionEvent> UnhighlightCallback;
        public Action<InteractionEvent> PressCallback;
        public Action<InteractionEvent> ReleaseCallback;
        public Action<InteractionEvent> CancelCallback;
        public Action<InteractionEvent> MoveCallback;

        public void Hover(InteractionEvent interactionEvent) => HighlightCallback?.Invoke(interactionEvent);
        public void UnHover(InteractionEvent interactionEvent) => UnhighlightCallback?.Invoke(interactionEvent);
        public void Press(InteractionEvent interactionEvent) => PressCallback?.Invoke(interactionEvent);
        public void Release(InteractionEvent interactionEvent) => ReleaseCallback?.Invoke(interactionEvent);
        public void Cancel(InteractionEvent interactionEvent) => CancelCallback?.Invoke(interactionEvent);
        public void Move(InteractionEvent interactionEvent) => MoveCallback?.Invoke(interactionEvent);
    }
}