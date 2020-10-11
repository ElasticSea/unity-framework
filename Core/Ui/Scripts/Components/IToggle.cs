namespace Core.Ui.Components
{
    public interface IToggle : IClickable
    {
        bool Selected { get; set; }
    }
}