using UnityEngine;

namespace Core.Ui.Components.Formatters
{
    public abstract class SelectFormatter : MonoBehaviour
    {
        public abstract void OnSelected(bool selected);
    }
}