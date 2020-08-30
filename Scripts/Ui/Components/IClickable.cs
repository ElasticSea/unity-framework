using System;

namespace _Framework.Scripts.Ui.Components
{
    public interface IClickable
    {
        event Action OnClick;
    }
}