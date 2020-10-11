using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Util.Callbacks
{
    public class OnPointerEnterCallback : MonoBehaviour, IPointerEnterHandler
    {
        public event Action<PointerEventData> OnPointerEnterEvent = data => { };
        public void OnPointerEnter(PointerEventData eventData) => OnPointerEnterEvent(eventData);
    }
}