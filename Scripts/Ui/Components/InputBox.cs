using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using _Framework.Scripts.Ui.Binding;
using _Framework.Scripts.Ui.Components.Validators;

namespace _Framework.Scripts.Ui.Components
{
    public class InputBox : MonoBehaviour, IUiBinding<string>
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Validator[] validators;
        [SerializeField] private ErrorPresenter[] presenters;
        [SerializeField] private bool readOnly;
        private bool ignore;

        private string value;
        public string Value
        {
            get => value;
            set
            {
                ignore = true;
                this.value = value;
                inputField.text = value;
                ignore = false; 
            }
        }

        public bool ReadOnly
        {
            get => inputField.readOnly;
            set => inputField.readOnly = value;
        }

        public event Action<string> OnValueChanged = f => { };

        private void Awake()
        {
            inputField.readOnly = readOnly;
            
            inputField.onValueChanged.AddListener(v =>
            {
                if (readOnly || ignore) return;

                var results = validators.Select(validator => validator.Validate(v)).ToList();
                var error = results.FirstOrDefault(result => result.IsValid ==false);
                var ok = results.FirstOrDefault(result => result.IsValid) ?? new ValidationResult{IsValid = true};
                if (error!=null)
                {
                    foreach (var errorPresenter in presenters)
                    {
                        errorPresenter.PresentError(error);
                    }
                }
                else
                {
                    foreach (var errorPresenter in presenters)
                    {
                        errorPresenter.PresentError(ok);
                    }

                    Value = v;
                    OnValueChanged(Value);
                }
            });
        }

        private object cachedGenericSwitchbox;

        public InputBox<T> As<T>(Func<string, T> from, Func<T, string> to)
        {
            if (cachedGenericSwitchbox == null)
            {
                cachedGenericSwitchbox = new InputBox<T>(this, @from, to);
            }
            // ReSharper disable once TryCastAndCheckForNull.0
            else if (cachedGenericSwitchbox as InputBox<T> == null)
            {
                throw new InvalidOperationException("Switchbox was cached as another type already.");
            }

            return (InputBox<T>) cachedGenericSwitchbox;
        }
        
        public InputBox<float> AsFloat()
        {
            var uiBinding = As(s => s.ToFloat(0).Value, f => f.ToString(CultureInfo.InvariantCulture));
            uiBinding.BackingField.AddValidator(new IsNumValidator());
            return uiBinding;
        }

        public InputBox<int> AsInt()
        {
            var uiBinding = As(s => s.ToInt(0).Value, f => f.ToString(CultureInfo.InvariantCulture));
            uiBinding.BackingField.AddValidator(new IsNumValidator());
            return uiBinding;
        }

        public void AddValidator(Validator validator)
        {
            validators = validators.Append(validator).ToArray();
        }
        
        public T AddValidator<T>() where T : Validator
        {
            var validator = gameObject.AddComponent<T>();
            validators = validators.Append(validator).ToArray();
            return validator;
        }

        public void Focus() => inputField.Select();
    }

    public class InputBox<T> : IUiBinding<T>
    {
        private InputBox inputBox;
        private Func<string, T> from;
        private Func<T, string> to;

        public InputBox(InputBox inputBox,Func<string, T> from, Func< T, string> to)
        {
            this.inputBox = inputBox;
            this.from = from;
            this.to = to;
            inputBox.OnValueChanged += s => OnValueChanged(Value);
        }

        public T Value
        {
            get => from(inputBox.Value);
            set
            {
                if (Equals(from(inputBox.Value), value) == false)
                {
                    inputBox.Value = to(value);
                }
            }
        }

        public InputBox BackingField => inputBox;

        public event Action<T> OnValueChanged = obj => {};

        public InputBox<T> With(Validator validator)
        {
            BackingField.AddValidator(validator);
            return this;
        }
    }
}