using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Framework.Scripts.Extensions;
using _Framework.Scripts.Ui.Binding;

namespace _Framework.Scripts.Ui.Components
{
    public class ListEditor : MonoBehaviour, IUiBinding<IEnumerable<object>>
    {
        [SerializeField] private ListView listView;
        [SerializeField] private Button addButton;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button editButton;
        [SerializeField] private Button moveUpButton;
        [SerializeField] private Button moveDownButton;

        public event Action<Action<object>> OnCreateEvent = action => { };
        public event Action<object, Action<object>> OnEditEvent = (o, action) => { };
        public event Action<IEnumerable<object>> OnDeletedEvent = obj => { };
        
        private void Awake()
        {
            addButton.OnClick += () =>
            {
                OnCreateEvent(o =>
                {
                    Value = Value.Append(o).ToList();
                    OnValueChanged(Value);
                });
            };
            deleteButton.OnClick += () =>
            {
                var selected = listView.Values.Value.ToSet();
                OnDeletedEvent(selected);
                Value = Value.Where(i => selected.Contains(i) == false).ToList();
                OnValueChanged(Value);
            };
            editButton.OnClick += () =>
            {
                var selected = listView.Value.Value;
                var index = listView.List.Value.IndexOf(selected);
                
                OnEditEvent(selected, o =>
                {
                    var list = listView.List.Value.ToList();
                    list[index] = o;
                    listView.List.Value = list;
                    OnValueChanged(Value);
                });
            };
            moveUpButton.OnClick += () =>
            {
                var selected = listView.Value.Value;

                if (selected != null)
                {
                    var index = listView.List.Value.IndexOf(selected);
                    var list = listView.List.Value.ToList();
                    if (index > 0)
                    {
                        list[index] = list[index - 1];
                        list[index - 1] = selected;
                        listView.List.Value = list;
                        listView.Value.Value = selected;
                        OnValueChanged(Value);
                    }
                }
            };
            moveDownButton.OnClick += () =>
            {
                var selected = listView.Value.Value;

                if (selected != null)
                {
                    var index = listView.List.Value.IndexOf(selected);
                    var list = listView.List.Value.ToList();
                    if (index < list.Count - 1)
                    {
                        list[index] = list[index + 1];
                        list[index + 1] = selected;
                        listView.List.Value = list;
                        listView.Value.Value = selected;
                        OnValueChanged(Value);
                    }
                }
            };
        }

        public IEnumerable<object> Value
        {
            get => listView.List.Value;
            set => listView.List.Value = value;
        }

        public bool AddButtonEnabled 
        {
            get => addButton.gameObject.activeSelf;
            set => addButton.gameObject.SetActive(value);
        }

        public bool DeleteButtonEnabled 
        {
            get => deleteButton.gameObject.activeSelf;
            set => deleteButton.gameObject.SetActive(value);
        }

        public bool EditButtonEnabled 
        {
            get => editButton.gameObject.activeSelf;
            set => editButton.gameObject.SetActive(value);
        }

        public event Action<IEnumerable<object>> OnValueChanged = objects => { };
        
        
        private object cached;

        public ListEditor<T> As<T>()
        {
            if (cached == null)
            {
                cached = new ListEditor<T>(this);
            }
            // ReSharper disable once TryCastAndCheckForNull.0
            else if (cached as ListEditor<T> == null)
            {
                throw new InvalidOperationException("ListEditor was cached as another type already.");
            }

            return (ListEditor<T>) cached;
        }
    }
    public class ListEditor<T>
    {
        public IUiBinding<IEnumerable<T>> List;
            
        public event Action<Action<T>> OnCreateEvent = action => { };
        public event Action<T, Action<T>> OnEditEvent = (o, action) => { };
        public event Action<IEnumerable<T>> OnDeletedEvent = obj => { };

        public ListEditor(ListEditor value)
        {
            List = BindingUtils.WrapBinding<T>(value);

            value.OnCreateEvent += action => OnCreateEvent(obj => action(obj));
            value.OnEditEvent += (o, action) => OnEditEvent(o is T arg1 ? arg1 : default, obj => action(obj));
            value.OnDeletedEvent += objects => OnDeletedEvent(objects.Cast<T>());
        }
    }
}