using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.Ui.Binding;
using Core.Util.Callbacks;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components
{
    public class FilterDialog : MonoBehaviour, IUiBinding<IEnumerable<object>>
    {
        [SerializeField] private int limit = 100;
        [SerializeField] private ListView listView;
        [SerializeField] private SearchBox searchBox;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject background;
        [SerializeField] private LayoutElement heightContributor;
        [SerializeField] private RectTransform widthContributor;

        public Func<object, string> FilterFunc;
        
        public event Action<object> OnSelected = s => { };
        public event Action OnClosed = () => { };
        
        private void Start()
        {
            var searchBoxProp = new PropertyBinding<string>();
            searchBoxProp.BindProperty(searchBox);
            
            listViewProp = new PropertyBinding<IEnumerable<object>>(() => Filter(searchBoxProp.Value));
            listView.OnItemClicked += o => OnSelected(o);
            listViewProp.BindProperty(listView.List);

            listViewProp.DependsOn(searchBoxProp);

            closeButton.OnClick += () => CloseIt();
            background.GetOrAddComponent<OnPointerClickCallback>().OnPointerClickEvent += data => CloseIt();
            
            searchBox.Focus();
        }

        private void CloseIt()
        {
            Destroy(gameObject);
            OnClosed();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                var candidate = listViewProp.Value.FirstOrDefault();
                if (candidate != null)
                {
                    OnSelected(candidate);
                }
            }
        }

        private IEnumerable<object> Filter(string text)
        {
            var filtered = Value
                .Where(value =>
                {
                    var toCheck = FilterFunc != null ? FilterFunc(value) : value.ToString();
                    return toCheck.Contains(text ?? "", StringComparison.InvariantCultureIgnoreCase);
                })
                .Take(Limit);

            return filtered;
        }

        public IEnumerable<object> Value
        {
            get { return _value ?? Enumerable.Empty<object>(); }
            set
            {
                _value = value;
                listViewProp?.TriggerUpdate();
            }
        }

        public ListViewItem itemPrefab
        {
            get { return listView.SavedPrefab; }
            set { listView.SavedPrefab = value; }
        }

        public float Height
        {
            get => heightContributor.preferredHeight;
            set => heightContributor.preferredHeight = value;
        }

        public float Width
        {
            get => widthContributor.GetWidth();
            set => widthContributor.SetWidth(value);
        }

        public int Limit
        {
            get => limit;
            set => limit = value;
        }

        public float SearchDelay
        {
            get => searchBox.SearchDelay;
            set => searchBox.SearchDelay = value;
        }

        public event Action<IEnumerable<object>> OnValueChanged;
        
        
        private object cachedGenericSwitchbox;
        private IEnumerable<object> _value;
        private PropertyBinding<IEnumerable<object>> listViewProp;

        public FilterDialog<T> As<T>()
        {
            if (cachedGenericSwitchbox == null)
            {
                cachedGenericSwitchbox = new FilterDialog<T>(this);
            }
            // ReSharper disable once TryCastAndCheckForNull.0
            else if (cachedGenericSwitchbox as FilterDialog<T> == null)
            {
                throw new InvalidOperationException("FilterDialog was cached as another type already.");
            }

            return (FilterDialog<T>) cachedGenericSwitchbox;
        }
    }

    public class FilterDialog<T>
    {
        public IUiBinding<IEnumerable<T>> List;
        public event Action<T> OnSelected = obj => { };
        public event Action OnClosed = () => { };
        
        private FilterDialog backing;

        public FilterDialog(FilterDialog value)
        {
            this.backing = value;
            List = BindingUtils.WrapBinding<T>(value);
            value.OnSelected += obj => OnSelected((T) obj);
            value.OnClosed += () => OnClosed();
        }

        public float Height
        {
            get => backing.Height;
            set => backing.Height = value;
        }

        public float Width
        {
            get => backing.Width;
            set => backing.Width = value;
        }

        public int Limit
        {
            get => backing.Limit;
            set => backing.Limit = value;
        }

        public float SearchDelay
        {
            get => backing.SearchDelay;
            set => backing.SearchDelay = value;
        }

        public FilterDialog Backing => backing;
    }
}