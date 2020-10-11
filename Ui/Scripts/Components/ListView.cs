using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.Util;
using Ui.Binding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Ui.Components
{
    public class ListView : MonoBehaviour
    {
        public IUiBinding<object> Value;
        public IUiBinding<IEnumerable<object>> Values;
        public IUiBinding<IEnumerable<object>> List;
        [SerializeField] private bool allowSwitchOff = false;
        [SerializeField] private bool allowMultiple = false;
        [SerializeField] private Transform container;
        [SerializeField] private bool reverseChildOrder;
        [SerializeField] private bool dropEvent;
        [SerializeField] private bool pooled;

        [FormerlySerializedAs("SwitchBoxItemPrefab")] [SerializeField] private ListViewItem itemPrefab;
        private ListViewItem savedPrefab;

        private readonly Dictionary<object, IToggle> map = new Dictionary<object, IToggle>();
        private readonly Dictionary<IToggle, object> otherMap = new Dictionary<IToggle, object>();

        private object cachedGenericSwitchbox;
        public ToggleGroup group { get; private set; }
        
        public event Action<ListViewItem> OnItemInstantiated = o => { };
        public event Action<object> OnItemClicked = item => { };
        public event Action<object> OnItemDoubleClicked = item => { };
        public event Action<object, object, DragPosition> OnDropEvent = (o, o1, position) => { };
        public event Action<KeyCode, object> OnKeyDown = (code, o) => { };

        public bool AllowSwitchOff
        {
            get { return @group.AllowSwitchingOff; }
            set { @group.AllowSwitchingOff = value; }
        }

        public bool AllowMultiple
        {
            get { return @group.AllowMultiple; }
            set { @group.AllowMultiple = value; }
        }

        public ListViewItem SavedPrefab
        {
            get { return savedPrefab; }
            set
            {
                savedPrefab = value; 
                SavedPrefab.gameObject.SetActive(false);
            }
        }

        protected virtual void Awake()
        {
            if (SavedPrefab == null)
            {
                if (itemPrefab.gameObject.scene.name == null)
                {
                    savedPrefab = itemPrefab;
                }
                else
                {
                    savedPrefab = Instantiate(itemPrefab);
                    savedPrefab.gameObject.SetActive(false);
                }
            }

            if (pooled)
            {
                pool = new Pool<ListViewItem>(0, () => Instantiate(SavedPrefab), item =>
                {
                    item.gameObject.SetActive(true);
                    var parent = container ? container : transform;
                    var ddd = item.transform;
                    ddd.SetParent(parent, false);
                    ddd.localScale = Vector3.one;
                }, item =>
                {
                    item.transform.SetParent(null);
                    var dragDropItem = item.GetComponent<DragDropItem>();
                    if (dragDropItem)
                    {
                        dragDropItem.OnDropEvent -= OnDropEvent;
                        dragDropItem.enabled = false;
                    }

                    item.gameObject.SetActive(false);
                });
            }

            group = new ToggleGroup(allowMultiple, allowSwitchOff);
            Values = new ValuesProperty(group, this);
            List = new ListProperty(Values, group, this);
            Value = new ValueProperty(Values);
        }

        private IDictionary<object, IToggle> createMap()
        {
            if (transform.GetParents().Contains(container))
            {
                throw new InvalidOperationException("Switchbox children container can't be a parent of switchbox.");
            }
            
            var map = new Dictionary<object, IToggle>();
            var objects = List.Value.ToArray();
            
            var parent = container ? container : transform;

            var createdObjects = pooled ? FillUpPool(pool, parent, objects) : CreateObjects(parent, objects);
            
            for (var index = 0; index < objects.Length; index++)
            {
                var value = objects[index];
                var instance = createdObjects[index];
                instance.ListView = this;
                instance.EnableDragAndDrop = dropEvent;

                if (dropEvent)
                {
                    var dragDropItem = instance.gameObject.GetOrAddComponent<DragDropItem>();
                    dragDropItem.enabled = true;
                    dragDropItem.Item = instance;
                    dragDropItem.OnDropEvent = OnDropEvent;
                }

                OnItemInstantiated(instance);
                var toggle = instance.GetComponent<IToggle>();

                map.Add(value, toggle);
            }
            
            for (var index = 0; index < objects.Length; index++)
            {
                var value = objects[index];
                createdObjects[index].Value = value;
                createdObjects[index].Selected = false;
            }
            
            if (reverseChildOrder)
            {
                for (var i = 0; i < parent.childCount; i++)
                {
                    parent.GetChild(i).SetSiblingIndex(0);
                }
            }
            
            return map;
        }

        private ListViewItem[] CreateObjects(Transform parent, object[] objects)
        {
            parent.DestroyChildren();
            
            var listViewItems = objects.Select(o =>
            {
                var listViewItem = Instantiate(SavedPrefab, parent, false);
                listViewItem.gameObject.SetActive(true);
                return listViewItem;
            }).ToArray();
            
            return listViewItems;
        }

        private ListViewItem[] FillUpPool(Pool<ListViewItem> pool1, Transform parent, object[] objects)
        {
            var diff = parent.childCount - objects.Length;

            if (diff > 0)
            {
                var toDelete = parent.ChildrenEnumerable().Take(diff).ToList();
                
                for (var i = 0; i < toDelete.Count; i++)
                {
                    pool1.Put(toDelete[i].GetComponent<ListViewItem>());
                }
                
            }else if (diff < 0)
            {
                for (var i = 0; i < -diff; i++)
                {
                    pool1.Get();
                }
            }
            
            return parent.Children().Select(c => c.GetComponent<ListViewItem>()).ToArray();
        }

        private void OnValidate()
        {
            if (group != null)
            {
                group.AllowMultiple = allowMultiple;
                group.AllowSwitchingOff = AllowSwitchOff;
            }
        }

        private class ValueProperty : IUiBinding<object>
        {
            private IUiBinding<IEnumerable<object>> vprop;

            public ValueProperty(IUiBinding<IEnumerable<object>> vprop)
            {
                this.vprop = vprop;

                vprop.OnValueChanged += objects =>
                {
                    OnValueChanged(Value);
                };
            }

            public object Value
            {
                get { return vprop.Value.FirstOrDefault(); }
                set { vprop.Value = value != null ? new List<object> {value} : Enumerable.Empty<object>(); }
            }

            public event Action<object> OnValueChanged = obj => { };
        }

        private class ValuesProperty : IUiBinding<IEnumerable<object>>
        {
            private readonly ToggleGroup controller;
            private readonly ListView switchbox;

            public ValuesProperty(ToggleGroup controller, ListView switchbox)
            {
                this.controller = controller;
                this.switchbox = switchbox;

                controller.OnActiveTogglesChanged += obj =>
                {
                    OnValueChanged(Value);
                };
            }

            public IEnumerable<object> Value
            {
                get { return controller.Active.Select(v => switchbox.otherMap[v]).ToList(); }
                set
                {
                    controller.Active = value
                        .Where(v => switchbox.map.ContainsKey(v))
                        .Select(v => switchbox.map[v])
                        .ToSet();
                }
            }

            public event Action<IEnumerable<object>> OnValueChanged = obj => { };
        }

        private class ListProperty : IUiBinding<IEnumerable<object>>
        {
            private readonly ToggleGroup toggleGroup;

            private IEnumerable<object> _value;
            private ListView switchbox;
            private IUiBinding<IEnumerable<object>> vprop;

            public ListProperty(IUiBinding<IEnumerable<object>> vprop, ToggleGroup toggleGroup, ListView switchbox)
            {
                this.vprop = vprop;
                this.toggleGroup = toggleGroup;
                this.switchbox = switchbox;
            }

            public IEnumerable<object> Value
            {
                get { return _value; }
                set
                {
                    _value = value;

                    switchbox.map.Clear();
                    switchbox.otherMap.Clear();
                    foreach (var entry in switchbox.createMap())
                    {
                        switchbox.map.Add(entry.Key, entry.Value);
                        switchbox.otherMap.Add(entry.Value, entry.Key);
                    }

                    toggleGroup.Toggles = switchbox.map.Values.ToList();

//                    vprop.Value = prevActive;
                }
            }

            public event Action<IEnumerable<object>> OnValueChanged = objects => { };
        }
        
        private object cached;
        private Pool<ListViewItem> pool;

        public ListView<T> As<T>()
        {
            if (cached == null)
            {
                cached = new ListView<T>(this);
            }
            // ReSharper disable once TryCastAndCheckForNull.0
            else if (cached as ListView<T> == null)
            {
                throw new InvalidOperationException("FilterDialog was cached as another type already.");
            }

            return (ListView<T>) cached;
        }

        public void SelectPrevious() => SelectOffset(1);

        public void SelectNext() => SelectOffset(-1);

        public void SelectPageUp() => SelectOffset(ItemsOnPage);

        public void SelectPageDown() => SelectOffset(-ItemsOnPage);

        private int ItemsOnPage
        {
            get
            {
                var item = group.Toggles.FirstOrDefault();
                if (item != null)
                {
                    var itemRect = (item as ListViewItem).GetComponent<RectTransform>();
                    var scrollView = transform.GetComponentInParent<ScrollRect>();
                    if (scrollView)
                    {
                        var scrollRect = scrollView.GetComponent<RectTransform>();

                        return (int) (scrollRect.GetHeight() / itemRect.GetHeight());
                    }
                }

                return 0;
            }
        }

        public void SelectOffset(int offset)
        {
            var toggle = group.Active.FirstOrDefault();
            var index = toggle != null ? group.Toggles.IndexOf(toggle) : 0;
            index = Mathf.Clamp(index - offset, 0, group.Toggles.Count - 1);
            group.Active = new HashSet<IToggle> {group.Toggles[index]};
        }

        public void TriggerKeyDown(KeyCode vKey)
        {
            OnKeyDown(vKey, Value.Value);
        }

        public void TriggerClick(object value, PointerEventData data)
        {
            OnItemClicked(value);
            EventSystem.current.SetSelectedGameObject(gameObject, data);
        }

        public void TriggerDoubleClick(object value, PointerEventData data)
        {
            OnItemDoubleClicked(value);
            EventSystem.current.SetSelectedGameObject(gameObject, data);
        }

        public void RefreshSelected()
        {
            if (Value.Value != null)
            {
                var item = (map[Value.Value] as Component).GetComponent<ListViewItem>();
                item.Value = item.Value;
            }
        }
    }

    public class ListView<T>
    {
        // TODO Rename values to selected and switchbox to listview
        public IUiBinding<IEnumerable<T>> List;
        public IUiBinding<T> Value;
        public IUiBinding<IEnumerable<T>> Values;

        public event Action<T> OnItemClicked = obj => { };
        public event Action<T> OnItemDoubleClicked = obj => { };
        public event Action<T, T, DragPosition> OnDropEvent = (arg1, arg2, position) => { };
        public event Action<KeyCode, T> OnKeyDown = (code, arg2) => { };

        public ListView(ListView value)
        {
            List = BindingUtils.WrapBinding<T>(value.List);
            Value = BindingUtils.WrapBinding<T>(value.Value);
            Values = BindingUtils.WrapBinding<T>(value.Values);

            value.OnItemClicked += o => OnItemClicked((T) o);
            value.OnItemDoubleClicked += o => OnItemDoubleClicked((T) o);
            value.OnDropEvent += (o, o1, position) => OnDropEvent((T) o, (T) o1, position);
            value.OnKeyDown += (code, o) => OnKeyDown(code, (T) o);
        }
    }
}