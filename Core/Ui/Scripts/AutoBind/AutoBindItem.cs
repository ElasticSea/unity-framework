using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Ui.AutoBind
{
    public class AutoBindItem<T> : IAutoBindItem
    {
        public string Name { get; }
        public bool Expandable { get; }
        public Type Type { get; }
        private Action<T> Setter { get; }
        private Func<T> Getter { get; }
        public List<T> Options { get; }

        public T Value
        {
            get => Getter();
            set => Setter(value);
        }
        
        public AutoBindItem (string name) : this(name, true, default, default, default, default, default)
        {
        }

        public AutoBindItem (string name, Type type, Action<T> setter, Func<T> getter, List<T> options, ControlType control) :this(name, false, type, setter, getter, options, control)
        {
        }
        
        private AutoBindItem(string name, bool expandable, Type type, Action<T> setter, Func<T> getter, List<T> options, ControlType control)
        {
            Name = name;
            Expandable = expandable;
            Type = type;
            Setter = setter;
            Getter = getter;
            Options = options;
            ControlType = control;
        }

        public List<object> Values => Options.Cast<object>().ToList();
        public ControlType ControlType { get; }

        object IAutoBindItem.Value
        {
            get => Value;
            set => Value = (T) value;
        }
    }
}