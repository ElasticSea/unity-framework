using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Binding
{
    public class BoolToText : MonoBehaviour, IUiBinding<bool>
    {
        [SerializeField] private bool isOn;
        [SerializeField] private string on = "On";
        [SerializeField] private string off = "Off";
        [SerializeField] private Text text;

        private void Awake()
        {
            Value = false;
        }

        public bool Value
        {
            get => isOn;
            set
            {
                this.isOn = value;
                text.text = isOn ? on : off;
            }
        }

        public event Action<bool> OnValueChanged = b => { };
    }
}