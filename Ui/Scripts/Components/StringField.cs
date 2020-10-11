using System;
using Ui.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Components
{
    public class StringField : MonoBehaviour, IUiBinding<string>
    {
        [SerializeField] private InputField inputField;

        private bool ignoreCallback;

        private void Awake()
        {
            inputField.onValueChanged.AddListener(v =>
            {
                if(ignoreCallback == false) OnValueChanged(Value);
            });
        }

        public string Value
        {
            get { return inputField.text; }
            set
            {
                ignoreCallback = true;
                inputField.text = value;
                ignoreCallback = false;
            }
        }

        public event Action<string> OnValueChanged = value => { };
    }
}