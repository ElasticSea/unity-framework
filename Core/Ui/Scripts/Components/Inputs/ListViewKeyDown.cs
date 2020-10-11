using Core.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Ui.Components.Inputs
{
    public class ListViewKeyDown : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private ListView listView;

        private bool Focused;
        private void Update()
        {
            if (Focused)
            {
                if (Input.anyKeyDown)
                {
                    foreach (var vKey in Utils.GetEnumValues<KeyCode>())
                    {
                        if (Input.GetKeyDown(vKey))
                        {
                            listView.TriggerKeyDown(vKey);
                        }
                    }
                }
            }
        }
        public void OnSelect(BaseEventData eventData) => Focused = true;
        public void OnDeselect(BaseEventData eventData)=> Focused = false;
    }
}