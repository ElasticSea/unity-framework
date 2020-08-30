using System;

namespace _Framework.Scripts.Ui.Binding
{
    public class UiBinding<T> : IUiBinding<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;

        public UiBinding(Func<T> getter = null, Action<T> setter = null, T value = default)
        {
            this.getter = getter;
            this.setter = setter;
            this.value = value;
        }

        private T value;
        public T Value
        {
            get => getter != null ? getter() : value;
            set
            {
                this.value = value;
                setter?.Invoke(value);
            }
        }

        public event Action<T> OnValueChanged = obj => { };
        public void Trigger() => OnValueChanged(Value);
    }
}