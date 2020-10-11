using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components.Formatters
{
    public class GameObjectFormatter : ValueFormatter
    {
        [SerializeField] private bool uppercase;
        [SerializeField] private Text label;

        public override void OnValueChanged(object o)
        {
            label.text = uppercase ? (o as GameObject).name.ToUpper() : (o as GameObject).name;
        }
    }
}