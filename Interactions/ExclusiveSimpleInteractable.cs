namespace ElasticSea.Framework.Interactions
{
    public class ExclusiveSimpleInteractable : IInteractable
    {
        private readonly IInteractable interactable;
        private object handLock;

        public ExclusiveSimpleInteractable(IInteractable interactable)
        {
            this.interactable = interactable;
        }

        public void Hover(InteractionEvent evnt) => interactable?.Hover(evnt);
        public void UnHover(InteractionEvent evnt) => interactable?.UnHover(evnt);

        public void Press(InteractionEvent evnt)
        {
            if (handLock == null)
            {
                handLock = evnt.InteractionProvider;
                interactable?.Press(evnt);
            }
        }

        public void Release(InteractionEvent evnt)
        {
            if (handLock == evnt.InteractionProvider)
            {
                handLock = null;
                interactable?.Release(evnt);
            }
        }

        public void Cancel(InteractionEvent evnt)
        {
            if (handLock == evnt.InteractionProvider)
            {
                handLock = null;
                interactable?.Cancel(evnt);
            }
        }

        public void Move(InteractionEvent evnt)
        {
            if (handLock == evnt.InteractionProvider)
            {
                interactable?.Move(evnt);
            }
        }
    }
}