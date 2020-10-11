using UnityEngine;

namespace Core.Ui.Components.Formatters
{
    public abstract class ValueFormatter : MonoBehaviour
    {
        public abstract void OnValueChanged(object value);
    }
}