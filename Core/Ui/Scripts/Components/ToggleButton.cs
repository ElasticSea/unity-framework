using System;
using Core.Ui.Binding;
using Core.Ui.Components.Formatters;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Ui.Components
{
    public class ToggleButton : ToggleBase, IUiBinding<bool>, IPointerClickHandler
    {
        [SerializeField] private SelectFormatter[] selectFormatters;

        private bool value;

        public bool Value
        {
            get => value;
            set
            {
                this.value = value;
                
                foreach (var formatter in selectFormatters)
                {
                    formatter.OnSelected(Selected);
                }
            }
        }

        public event Action<bool> OnValueChanged = b => {};

        public void OnPointerClick(PointerEventData eventData)
        {
            Value = !Value;
            OnValueChanged(Value);
            TriggerClickEvent();
        }

        public override bool Selected
        {
            get { return Value; }
            set { Value = value; }
        }
    }
}