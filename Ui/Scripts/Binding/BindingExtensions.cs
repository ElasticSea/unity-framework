using System;
using System.Collections.Generic;
using Core.Util;
using Ui.Components;

namespace Ui.Binding
{
    public static class BindingExtensions
    {
        public static IUiBinding<TTo> Filter<TFrom, TTo>(this IUiBinding<TFrom> binding, Func<TFrom,TTo> getter, Func<TTo, TFrom> setter)
        {
            return new UiBindingFilter<TTo, TFrom>(binding, getter, setter);
        }

        public static IUiBinding<float> ToFloat(this IUiBinding<int> binding)
        {
            return new UiBindingFilter<float, int>(binding, i => (float) i, f => (int) f);
        }

        public static IUiBinding<int> ToInt(this IUiBinding<float> binding)
        {
            return new UiBindingFilter<int, float>(binding, i => (int) i, f => (float) f);
        }

        public static PropertyBinding<T> SimpleBind<T>(this ListView listView, IEnumerable<T> values)
        {
            var casted = listView.As<T>();
            casted.List.Value = values;
            
            var prop = new PropertyBinding<T>();
            prop.BindProperty(casted.Value);
            return prop;
        }

        public static PropertyBinding<T> EnumBind<T>(this ListView listView) where T : Enum
        {
            return listView.SimpleBind(Utils.GetEnumValues<T>());
        }
        
        public static PropertyBinding<T> SimpleBind<T>(this Combobox combobox, IEnumerable<T> values,  PropertyBinding<T> binding = null)
        {
            var casted = combobox.As<T>();
            casted.List.Value = values;

            var prop = binding ?? new PropertyBinding<T>();
            prop.BindProperty(casted.Selected);
            return prop;
        }

        public static PropertyBinding<T> EnumBind<T>(this Combobox combobox,  PropertyBinding<T> binding = null) where T : Enum
        {
            return combobox.SimpleBind(Utils.GetEnumValues<T>(), binding);
        }
    }
}