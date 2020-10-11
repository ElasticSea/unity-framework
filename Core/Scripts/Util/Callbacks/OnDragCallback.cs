using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Util.Callbacks
{
    public class OnDragCallback : MonoBehaviour, IDragHandler
    {
        public event Action<PointerEventData> OnDragEvent = data => { };
        public void OnDrag(PointerEventData eventData) => OnDragEvent(eventData);
    }
}