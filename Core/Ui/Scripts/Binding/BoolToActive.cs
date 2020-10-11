using System;
using UnityEngine;

namespace Core.Ui.Binding
{
    public class BoolToActive : MonoBehaviour, IUiBinding<bool>
    {
        [SerializeField] private bool isOn;
        [SerializeField] private bool inverted;
        [SerializeField] private GameObject target;

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
                target.SetActive(Value != inverted);
            }
        }

        public event Action<bool> OnValueChanged = b => { };
    }
}