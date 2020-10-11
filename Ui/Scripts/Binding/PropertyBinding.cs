using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Ui.Binding
{
    public class PropertyBinding<T> : IPropertyBinding<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;

        public bool Enabled { get; }

        private readonly Dictionary<IUiBinding<T>, Binder<T>> binders = new Dictionary<IUiBinding<T>, Binder<T>>();
        
        public PropertyBinding(Func<T> getter = null, Action<T> setter = null, bool enabled = true, T value = default)
        {
            this.getter = getter;
            this.setter = setter;
            // TODO Meh.. could replace this with another nested boolean PropertyBinding
            Enabled = enabled;
            this.value = value;
        }

        private T value;
        public T Value
        {
            get
            {
                return getter != null ? getter() : value;
            }
            set
            {
                if (Enabled)
                {
                    this.value = value;
                    setter?.Invoke(value);
                    TriggerUpdate();
                }
            }
        }

        private Action<T> OnValueChangedField = obj => { };

        public event Action<T> OnValueChanged
        {
            add { OnValueChangedField = value + OnValueChangedField; }
            remove { OnValueChangedField -= value; }
        }

        public IPropertyBinding<T> DependsOn<B>(IPropertyBinding<B> triggers)
        {
            triggers.OnValueChanged += b =>
            {
                try
                {
                    Value = Value;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            };
            return this;
        }

        public PropertyBinding<T> BindProperty(IUiBinding<T> ui)
        {
            Action<T> propCallback = value => ui.Value = value;
            Action<T> uiCallback = value =>
            {
                if (DynamicEquals(Value, value) == false)
                {
                    Value = value;
                }
            };

            binders.Add(ui, new Binder<T> {propCallback = propCallback, uiCallbac = uiCallback});
            OnValueChanged += propCallback;
            ui.OnValueChanged += uiCallback;
            ui.Value = Value;

            return this;
        }

        public void BindValue(Action<T> callback)
        {
            OnValueChanged += value => callback(value);
            callback(value);
        }

        public void RemoveBinding(IUiBinding<T> ui)
        {
            if (binders.ContainsKey(ui))
            {
                var binder = binders[ui];
                OnValueChanged -= binder.propCallback;
                ui.OnValueChanged -= binder.uiCallbac;
                binders.Remove(ui);
            }
        }

        private class Binder<T>
        {
            public Action<T> propCallback;
            public Action<T> uiCallbac;
        }

        public void TriggerUpdate()
        {
            OnValueChangedField(getter != null ? getter() : value);
        }
        
        public static bool DynamicEquals<T>(T value, T other)
        {
            if (Equals(value, other))
            {
                return true;
            }

            if (Equals(value, default(T)) || Equals(other, default(T)))
            {
                return false;
            }

            if (value is IEnumerable && other is IEnumerable && (other is string == false) && (other is Array == false))
            {
                var t = typeof(Enumerable);
                var meth = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Single(m => m.Name.Equals("SequenceEqual") && m.GetParameters().Count() == 2);
                var gm = meth.MakeGenericMethod(typeof(T).GetGenericArguments().Single());
                return (bool)gm.Invoke(null, new object[] { value, other });
            }

            return value.Equals(other);
        }
        
    }
}