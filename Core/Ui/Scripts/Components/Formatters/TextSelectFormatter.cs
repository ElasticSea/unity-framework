using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components.Formatters
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