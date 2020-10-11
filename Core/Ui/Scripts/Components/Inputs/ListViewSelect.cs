using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Ui.Components.Inputs
{
    public class ListViewSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private ListView listView;
        [SerializeField] private float selectTriggerDelay = .25f;
        [SerializeField] private float selectDelay = .05f;

        private bool Focused;
        
        private float upKeyDownTriggerTime;
        private float upKeyDownTriggerSelectTime;
        private float downKeyDownTriggerTime;
        private float downKeyDownTriggerSelectTime;

        private void Update()
        {
            if (Focused)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    upKeyDownTriggerTime = Time.time + selectTriggerDelay;
                    listView.SelectPrevious();
                }

                if (Input.GetKey(KeyCode.UpArrow))
                {
                    if (Time.time >= upKeyDownTriggerTime)
                    {
                        while (Time.time >= upKeyDownTriggerSelectTime)
                        {
                            listView.SelectPrevious();
                            upKeyDownTriggerSelectTime = Time.time + selectDelay;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    downKeyDownTriggerTime = Time.time + selectTriggerDelay;
                    listView.SelectNext();
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (Time.time >= downKeyDownTriggerTime)
                    {
                        while (Time.time >= downKeyDownTriggerSelectTime)
                        {
                            listView.SelectNext();
                            downKeyDownTriggerSelectTime = Time.time + selectDelay;
                        }
                    }
                }
                
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    listView.SelectPageUp();
                }

                if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    listView.SelectPageDown();
                }
            }
        }
        public void OnSelect(BaseEventData eventData) => Focused = true;
        public void OnDeselect(BaseEventData eventData)=> Focused = false;
    }
}