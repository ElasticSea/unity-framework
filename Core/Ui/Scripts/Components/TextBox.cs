using System;
using System.Globalization;
using Core.Ui.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components
{
    public class TextBox : MonoBehaviour, IUiBinding<string>
    {
        [SerializeField] private Text text;

        public string Value
        {
            get { return text.text; }
            set { this.text.text = value; }
        }

        public event Action<string> OnValueChanged;

        private object cachedGenericSwitchbox;

        public TextBox<T> As<T>(Func<string, T> from = null, Func<T, string> to = null)
        {
            if (cachedGenericSwitchbox == null)
            {
                cachedGenericSwitchbox = new TextBox<T>(this, @from, to);
            }
            // ReSharper disable once TryCastAndCheckForNull.0
            else if (cachedGenericSwitchbox as TextBox<T> == null)
            {
                throw new InvalidOperationException("TextBox was cached as another type already.");
            }

            return (TextBox<T>) cachedGenericSwitchbox;
        }
        
        public TextBox<float> AsFloat()
        {
            return As(s => s.ToFloat(), f => f.ToString(CultureInfo.InvariantCulture));
        }

        public TextBox<int> AsInt()
        {
            return As(s => s.ToInt(), f => f.ToString(CultureInfo.InvariantCulture));
        }
    }
    
    public class TextBox<T> : IUiBinding<T>
    {
        private TextBox inputBox;
        private Func<string, T> from;
        private Func<T, string> to;

        public TextBox(TextBox inputBox,Func<string, T> from, Func< T, string> to)
        {
            this.inputBox = inputBox;
            this.from = from;
            this.to = to;
        }

        public T Value
        {
            get => from(inputBox.Value);
            set => inputBox.Value = to(value);
        }

        public TextBox BackingField => inputBox;

        public event Action<T> OnValueChanged = obj => {};
    }
}