using System;

namespace Core.Ui.Binding
{
    public class UiBindingFilter<TTo, TFrom> : IUiBinding<TTo>
    {
        private readonly Func<TFrom, TTo> getter;
        private readonly Func<TTo, TFrom> setter;
        private readonly IUiBinding<TFrom> binding;

        public TTo Value
        {
            get => getter(binding.Value);
            set => binding.Value = setter(value);
        }

        public UiBindingFilter(IUiBinding<TFrom> binding, Func<TFrom,TTo> getter, Func<TTo, TFrom> setter)
        {
            this.binding = binding;
            binding.OnValueChanged += from => OnValueChanged(Value);
            
            this.getter = getter;
            this.setter = setter;
        }

        public event Action<TTo> OnValueChanged = to => { };
    }
}