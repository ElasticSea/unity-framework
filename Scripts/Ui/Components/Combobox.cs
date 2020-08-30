using System;
using System.Collections.Generic;
using System.Linq;
using _Framework.Scripts.Extensions;
using _Framework.Scripts.Ui.Binding;
using _Framework.Scripts.Ui.Components.Formatters;
using _Framework.Scripts.Util.Callbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Framework.Scripts.Ui.Components
{
    public class Combobox : MonoBehaviour
    {
        [SerializeField] private ListView comboViewPrefab;
        [SerializeField] private Text label;

        private ListView list;
        private GameObject background;
        private ListBinding listBinding;
        private SelectedBinding selectedBinding;

        private Func<object, string> textFormatter;

        public Func<object, string> TextFormatter
        {
            set => textFormatter = value;
        }

        public IUiBinding<IEnumerable<object>> List => listBinding;
        public IUiBinding<object> Selected => selectedBinding;

        public class ListBinding : IUiBinding<IEnumerable<object>>
        {
            public IEnumerable<object> Value { get; set; }
            public event Action<IEnumerable<object>> OnValueChanged;
        }

        public class SelectedBinding : IUiBinding<object>
        {
            private object value;
            private Text label;
            private ListBinding listBinding;
            private Combobox combobox;

            public SelectedBinding(Text label, ListBinding listBinding, Combobox combobox)
            {
                this.label = label;
                this.listBinding = listBinding;
                this.combobox = combobox;
            }

            public object Value
            {
                get => value;
                set
                {
                    this.value = value;
                    if (listBinding.Value.Contains(value))
                    {
                        label.text = (combobox.textFormatter?.Invoke(value) ?? value?.ToString())?.ToUpper();
                        listBinding.Value = listBinding.Value.OrderByDescending(v => v == value).ToArray();
                    }
                    else
                    {
                        this.value = listBinding.Value.First();
                        OnValueChanged(Value);
                    }
                }
            }

            public event Action<object> OnValueChanged = s => { };

            public void TriggerChange()
            {
                OnValueChanged(Value);
            }
        }

        private void Awake()
        {
            listBinding = new ListBinding();
            selectedBinding = new SelectedBinding(label, listBinding, this);

            GetComponent<Button>().OnClick += () =>
            {
                var canvas = transform.GetComponentInParent<Canvas>();

                background = CreateBackground(canvas);
                list = CreateList(canvas);
            };
        }

        private ListView CreateList(Canvas canvas)
        {
            var list = Instantiate(comboViewPrefab, canvas.transform, true);
            list.OnItemInstantiated += item =>
            {
                item.GetComponent<TextFormatter>().Overrided = textFormatter;
            };

            var thisRt = transform.GetComponent<RectTransform>();
            var listRt = list.GetComponent<RectTransform>();

            listRt.anchoredPosition = thisRt.anchoredPosition;
            listRt.pivot = new Vector2(0.5f, 1);
            list.transform.position = transform.position;
            list.transform.rotation = transform.rotation;
            list.transform.localScale = transform.localScale;

            var size = thisRt.GetSize();
            listRt.SetSize(size.x, size.y * List.Value.Count());

            list.As<object>().List.Value = List.Value;
            list.As<object>().Value.Value = Selected.Value;

            list.As<object>().OnKeyDown += (code, o) =>
            {
                if (code == KeyCode.Return)
                {
                    SelectIt(o);
                }
            };

            list.As<object>().OnItemClicked += s =>
            {
                SelectIt(s);
            };

            list.gameObject.Exectute(ExecuteEvents.selectHandler);

            return list;
        }

        private GameObject CreateBackground(Canvas canvas)
        {
            var background = new GameObject("Background");
            var backgroundRT = background.AddComponent<RectTransform>();
            background.transform.SetParent(canvas.transform, false);
            backgroundRT.anchorMin = Vector2.zero;
            backgroundRT.anchorMax = Vector2.one;
            backgroundRT.offsetMin = Vector2.zero;
            backgroundRT.offsetMax = Vector2.zero;

            background.AddComponent<LayoutElement>().ignoreLayout = true;
            background.AddComponent<Image>().color = Color.clear;

            background.AddComponent<OnPointerClickCallback>().OnPointerClickEvent += data =>
            {
                CloseDialog();
            };
            return background;
        }

        private void SelectIt(object o)
        {
            Selected.Value = o;
            selectedBinding.TriggerChange();
            CloseDialog();
        }

        private void CloseDialog()
        {
            Destroy(background);
            Destroy(list.gameObject);
        }

        private object cached;

        public Combobox<T> As<T>()
        {
            if (cached == null)
            {
                cached = new Combobox<T>(this);
            }
            // ReSharper disable once TryCastAndCheckForNull.0
            else if (cached as Combobox<T> == null)
            {
                throw new InvalidOperationException(
                    $"You are trying to cache {nameof(Combobox)} as [{typeof(T)}] when it was cached as [{cached.GetType()}] already.");
            }

            return (Combobox<T>) cached;
        }
    }

    public class Combobox<T>
    {
        // TODO Rename values to selected and switchbox to listview
        public IUiBinding<IEnumerable<T>> List;
        public IUiBinding<T> Selected;

        private Combobox backingCombobox;

        public Func<T, string> TextFormatter
        {
            set => backingCombobox.TextFormatter = o => value(o is T ? (T) o : default);
        }

        public Combobox(Combobox backingCombobox)
        {
            this.backingCombobox = backingCombobox;
            if (backingCombobox.List == null && backingCombobox.Selected == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(Combobox)} is not ilitialized, this might happen, due to calling this methods from Awake.");
            }

            List = BindingUtils.WrapBinding<T>(backingCombobox.List);
            Selected = BindingUtils.WrapBinding<T>(backingCombobox.Selected);
        }
    }
}