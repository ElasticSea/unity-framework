using UnityEngine;

namespace Ui.Components.Formatters
{
    public abstract class SelectFormatter : MonoBehaviour
    {
        public abstract void OnSelected(bool selected);
    }
}