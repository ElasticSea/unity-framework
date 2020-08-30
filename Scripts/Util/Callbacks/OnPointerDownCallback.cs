using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Framework.Scripts.Util.Callbacks
{
    public class OnPointerDownCallback : MonoBehaviour, IPointerDownHandler
    {
        public event Action<PointerEventData> OnPointerDownEvent = data => { };
        public void OnPointerDown(PointerEventData eventData) => OnPointerDownEvent(eventData);
    }
}