using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Util.Callbacks
{
    public class OnPointerUpCallback : MonoBehaviour, IPointerUpHandler
    {
        public event Action<PointerEventData> OnPointerUpEvent = data => { };
        public void OnPointerUp(PointerEventData eventData) => OnPointerUpEvent(eventData);
    }
}