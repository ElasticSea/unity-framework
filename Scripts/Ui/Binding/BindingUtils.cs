using System;
using System.Collections.Generic;
using System.Linq;
using _Framework.Scripts.Ui.Components;
using _Framework.Scripts.Util;

namespace _Framework.Scripts.Ui.Binding
{
    public class BindingUtils
    {
        public static IUiBinding<T> WrapBinding<T>(IUiBinding<object> binding)
        {
            return new ValueProperty<T>(binding);
        }
        
        public static IUiBinding<IEnumerable<T>> WrapBinding<T>(IUiBinding<IEnumerable<object>> binding)
        {
            return new ValuesProperty<T>(binding);
        }

        public static PropertyBinding<T> BindListViewEnum<T>(ListView<T> listview)
        {
            var list = new PropertyBinding<IEnumerable<T>>(value:Utils.GetEnumValues<T>());
            var value = new PropertyBinding<T>();
            
            list.BindProperty(listview.List);
            value.BindProperty(listview.Value);

            return value;
        }
        
        private class ValuesProperty<T> : IUiBinding<IEnumerable<T>>
        {
            private readonly IUiBinding<IEnumerable<object>> binding;

            public ValuesProperty(IUiBinding<IEnumerable<object>> binding)
            {
                this.binding = binding;
                this.binding.OnValueChanged += o =>
                {
                    OnValueChanged(Value);
                };
            }

            public IEnumerable<T> Value
            {
                get { return binding.Value as IEnumerable<T> ?? binding.Value.Cast<T>(); }
                set { binding.Value = value as IEnumerable<object> ?? value?.Cast<object>() ?? Enumerable.Empty<object>(); }
            }

            public event Action<IEnumerable<T>> OnValueChanged = obj => { };
        }

        private class ValueProperty<T> : IUiBinding<T>
        {
            private readonly IUiBinding<object> binding;

            public ValueProperty(IUiBinding<object> binding)
            {
                this.binding = binding;
                this.binding.OnValueChanged += o =>
                {
                    OnValueChanged(Value);
                };
            }

            public T Value
            {
                get { return (T) (binding?.Value ?? default(T)); }
                set { binding.Value = value; }
            }

            public event Action<T> OnValueChanged = obj => { };
        }
    }
}