using System;
using UnityEngine;

namespace _Framework.Scripts.Ui.Binding
{
    public class BoolToVisible : MonoBehaviour, IUiBinding<bool>
    {
        [SerializeField] private bool isOn;
        [SerializeField] private bool inverted;
        [SerializeField] private CanvasGroup group;

        private void Awake()
        {
            Value = false;
        }

        public bool Value
        {
            get => isOn;
            set
            {
                isOn = value;
                group.alpha = Value == inverted ? 0 : 1;
            }
        }

        public event Action<bool> OnValueChanged = b => { };
    }
}