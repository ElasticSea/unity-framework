using System;

namespace _Framework.Scripts.Ui.Binding
{
    public interface IUiBinding<T>
    {
        T Value { get; set; }
        event Action<T> OnValueChanged;
    }
}