using UnityEngine;

namespace _Framework.Scripts.Ui.Components.Formatters
{
    public abstract class ValueFormatter : MonoBehaviour
    {
        public abstract void OnValueChanged(object value);
    }
}