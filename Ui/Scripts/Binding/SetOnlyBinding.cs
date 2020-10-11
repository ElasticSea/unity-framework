using System;

namespace Ui.Binding
{
    public class SetOnlyBinding<T> : IUiBinding<T>
    {
        private Action<T> callback;

        public T Value
        {
            get { throw new InvalidOperationException(); }
            set { this.callback(value); }
        }

        public SetOnlyBinding(Action<T> callback)
        {
            this.callback = callback;
        }

        public event Action<T> OnValueChanged;
    }
}