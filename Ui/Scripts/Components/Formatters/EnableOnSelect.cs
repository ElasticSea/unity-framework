using UnityEngine;

namespace Ui.Components.Formatters
{
    public class EnableOnSelect : SelectFormatter
    {
        [SerializeField] private bool inverted;
        [SerializeField] private GameObject go;
        
        public override void OnSelected(bool selected)
        {
            go.SetActive(selected == !inverted);
        }
    }
}