using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Components.Formatters
{
    public class TextFormatter : ValueFormatter
    {
        [SerializeField] private Text label;
        [SerializeField] private bool uppercase;

        private Func<object, string> overrided;

        public Func<object, string> Overrided
        {
            set => overrided = value;
        }
        
        public override void OnValueChanged(object o)
        {
            var txt = overrided?.Invoke(o) ?? o?.ToString() ?? "";
            
            if(label) label.text = uppercase ? txt.ToUpper() : txt.ToString();
        }
    }
}