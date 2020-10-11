using UnityEngine;
using UnityEngine.EventSystems;

namespace Ui.Components.Inputs
{
    public class ShiftTogglesBetweenMultiselect: MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private ListView listView;

        private bool Focused;

        private void Update()
        {
            if (Focused)
            {
                var shiftPressed = Input.GetKey(KeyCode.LeftShift) ||
                               Input.GetKey(KeyCode.RightShift);

                listView.group.PendingAllowMultiple = shiftPressed;
            }
        }
        public void OnSelect(BaseEventData eventData) => Focused = true;
        public void OnDeselect(BaseEventData eventData)=> Focused = false;
    }
}