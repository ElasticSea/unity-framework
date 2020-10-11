using System;

namespace Core.Ui.Components
{
    public interface IClickable
    {
        event Action OnClick;
    }
}