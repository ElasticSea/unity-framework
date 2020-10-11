using System;

namespace Core.Ui.Binding
{
    public interface IPropertyBinding<T>
    {
        T Value { get; set; }
        bool Enabled { get; }

        event Action<T> OnValueChanged;

        IPropertyBinding<T> DependsOn<B>(IPropertyBinding<B> triggers);
    }
}