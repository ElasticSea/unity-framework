namespace _Framework.Scripts.Ui.Components
{
    public interface IToggle : IClickable
    {
        bool Selected { get; set; }
    }
}