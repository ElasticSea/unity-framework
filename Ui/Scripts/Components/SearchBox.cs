using System;
using Ui.Binding;
using UnityEngine;

namespace Ui.Components
{
    public class SearchBox : MonoBehaviour, IUiBinding<string>
    {
        [SerializeField] private InputBox inputBox;
        [SerializeField] private int delayedCallbackMillis;
        private string searchFieldText;
        private float timestamp = -1;

        private void Awake()
        {
            inputBox.OnValueChanged += s =>
            {
                timestamp = Time.unscaledTime;
            };
        }

        private string value;

        private void Update()
        {
            if (timestamp != -1 && timestamp + delayedCallbackMillis / 1000f <= Time.unscaledTime)
            {
                timestamp = -1;
                OnValueChanged(Value);
            }
        }

        public string Value
        {
            get { return inputBox.Value; }
            set { inputBox.Value = value; }
        }

        public float SearchDelay
        {
            get => delayedCallbackMillis;
            set => delayedCallbackMillis = (int) (value * 1000);
        }

        public void Focus() => inputBox.Focus();

        public event Action<string> OnValueChanged = s => { };
    }
}