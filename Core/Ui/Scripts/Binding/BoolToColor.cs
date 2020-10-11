using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Binding
{
    public class BoolToColor : MonoBehaviour, IUiBinding<bool>
    {
        [SerializeField] private bool isOn;
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = Color.black;
        [SerializeField] private Graphic background;

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
                background.color = isOn ? onColor : offColor;
            }
        }

        public event Action<bool> OnValueChanged = b => { };
    }
}