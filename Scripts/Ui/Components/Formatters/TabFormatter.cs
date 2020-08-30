using UnityEngine;
using UnityEngine.UI;
using _Framework.Scripts.Extensions;

namespace _Framework.Scripts.Ui.Components.Formatters
{
    public class TabFormatter : ValueFormatter
    {
        [SerializeField] private Text icon;
        [SerializeField] private Text label;
        [SerializeField] private bool uppercase;

        public override void OnValueChanged(object o)
        {
            var tab = (o as Tab);
            
            if (icon)
            {
                icon.text = tab.Icon;
                icon.gameObject.SetActive(icon.text.IsNullOrEmpty() == false);
            }

            if (label)
            {
                label.text = uppercase ? tab.Text.ToUpper() : tab.Text;
                label.gameObject.SetActive(label.text.IsNullOrEmpty() == false);
            }
        }
    }
}