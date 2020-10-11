using System;

namespace Ui.Components
{
    public interface IClickable
    {
        event Action OnClick;
    }
}