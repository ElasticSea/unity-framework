using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Ui.Binding;
using Ui.Components.Formatters;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ui.Components
{
    public class Select : MonoBehaviour, IPointerClickHandler
    {
        public IUiBinding<IEnumerable<object>> List { get; private set; }
        public IUiBinding<object> Selected { get; private set; }
        public IUiBinding<bool> Enabled { get; private set; }

        [SerializeField] private FilterDialog filterDialogPrefab;
        [SerializeField] private GameObject arrowGraphic;
        [SerializeField] private ValueFormatter[] valueFormatters;
        [SerializeField] private ListViewItem itemPrefab;

        public Func<object, string> Filter;

        private bool open;
        private bool enabledOpening = true;
        private FilterDialog filterDialog;
        private bool isInitialized;
        
        private void Awake()
        {
            if (isInitialized == false)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            List = new ListBinding(this);
            Selected = new SelectedBinding(this);
            Enabled = new EnabledBinding(this);
            isInitialized = true;
        }

        private class EnabledBinding : IUiBinding<bool>
        {
            private bool value;
            private Select @select;

            public EnabledBinding(Select select)
            {
                this.select = select;
            }

            public bool Value
            {
                get { return value; }
                set
                {
                    this.value = value;
                    select.enabledOpening = value;
                    @select.arrowGraphic.SetActive(value);
                }
            }

            public event Action<bool> OnValueChanged = o => { };
        }

        private class ListBinding : IUiBinding<IEnumerable<object>>
        {
            private IEnumerable<object> value;
            private Select @select;

            public ListBinding(Select select)
            {
                this.select = select;
            }

            public IEnumerable<object> Value
            {
                get { return value; }
                set
                {
                    this.value = value;
                    select.FillDialog(value);
                    OnValueChanged(Value);
                }
            }

            public event Action<IEnumerable<object>> OnValueChanged = o => { };
        }

        private class SelectedBinding : IUiBinding<object>
        {
            private object value;
            private Select @select;

            public SelectedBinding(Select select)
            {
                this.select = select;
            }

            public object Value
            {
                get { return value; }
                set
                {
                    this.value = value;
                    select.FillField(value);
                    OnValueChanged(Value);
                }
            }

            public event Action<object> OnValueChanged = o => { };
        }

        private void FillField(object value)
        {
            foreach (var formatter in valueFormatters)
            {
                formatter.OnValueChanged(value);
            }
        }

        private void FillDialog(IEnumerable<object> value)
        {
            if (filterDialog)
            {
                filterDialog.Value = value.ToList();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enabledOpening && open == false && List.Value.Any())
            {
                filterDialog = Instantiate(filterDialogPrefab);
                filterDialog.FilterFunc = Filter;
                if (itemPrefab) filterDialog.itemPrefab = itemPrefab;
                filterDialog.GetComponent<Canvas>().sortingOrder = GetComponent<RectTransform>().GetParentCanvas().sortingOrder + 1;

                filterDialog.OnSelected += tuple =>
                {
                    Destroy(filterDialog.gameObject);
                    Selected.Value = tuple;
                    open = false;
                };
                filterDialog.OnClosed += () =>
                {
                    open = false;
                };
                FillDialog(List.Value);
                open = true;
            }
        }

        private object cachedGenericSelect;
        public Select<T> As<T>()
        {
            if (cachedGenericSelect == null)
            {
                cachedGenericSelect = new Select<T>(this);
            }
            // ReSharper disable once TryCastAndCheckForNull.0
            else if (cachedGenericSelect as Select<T> == null)
            {
                throw new InvalidOperationException("Select was cached as another type already.");
            }

            return (Select<T>)cachedGenericSelect;
        }
    }

    public class Select<T>
    {
        public IUiBinding<T> Value;
        public IUiBinding<IEnumerable<T>> List;
        
        private Select switchbox;

        public Select(Select switchbox)
        {
            this.switchbox = switchbox;
            Value = BindingUtils.WrapBinding<T>(switchbox.Selected);
            List = BindingUtils.WrapBinding<T>(switchbox.List);
        }

        public Func<T, string> Filter
        {
            set => switchbox.Filter = o => value(o is T ? (T) o : default);
        }
    }
}