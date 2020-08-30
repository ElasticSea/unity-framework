using UnityEngine;
using UnityEngine.UI;

namespace _Framework.Scripts.Ui.Components.Formatters
{
    public class TextSelectFormatter : SelectFormatter
    {
        [SerializeField] private Text label;
        [SerializeField] private string on;
        [SerializeField] private string off;

        public override void OnSelected(bool selected)
        {
            label.text = selected ? on : off;
        }
    }
}