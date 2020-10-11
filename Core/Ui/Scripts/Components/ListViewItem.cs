using System;
using Core.Ui.Components.Formatters;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Ui.Components
{
    public class ListViewItem : MonoBehaviour, IPointerClickHandler, IToggle
    {
        private object value;
        private bool selected;

        public event Action OnClick = () => { };

        protected void TriggerClickEvent() => OnClick();

        [SerializeField] private SelectFormatter[] selectFormatters;
        [SerializeField] private ValueFormatter[] valueFormatters;

        private void Start()
        {
            foreach (var formatter in selectFormatters)
            {
                formatter.OnSelected(Selected);
            }
        }

        public virtual object Value
        {
            get { return value; }
            set
            {
                this.value = value;

                foreach (var formatter in valueFormatters)
                {
                    formatter.OnValueChanged(Value);
                }
            }
        }

        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    foreach (var formatter in selectFormatters)
                    {
                        formatter.OnSelected(Selected);
                    }
                }
            }
        }

        public ListView ListView { get; set; }
        public bool EnableDragAndDrop { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            TriggerClickEvent();
            ListView.TriggerClick(Value, eventData);
            if (eventData.clickCount == 2)
            {
                ListView.TriggerDoubleClick(Value, eventData);
            }
        }
    }
}