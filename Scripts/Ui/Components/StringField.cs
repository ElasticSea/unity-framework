using System;
using UnityEngine;
using UnityEngine.UI;
using _Framework.Scripts.Ui.Binding;

namespace _Framework.Scripts.Ui.Components
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