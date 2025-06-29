namespace ElasticSea.Framework.Ui.Interactions
{
    public interface IInteractable
    {
        void Hover(InteractionEvent interactionEvent);
        void UnHover(InteractionEvent interactionEvent);
        void Press(InteractionEvent interactionEvent);
        void Release(InteractionEvent interactionEvent);
        void Cancel(InteractionEvent interactionEvent);
        void Move(InteractionEvent interactionEvent);
    }
}