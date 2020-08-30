using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Framework.Scripts.Util.Callbacks
{
    public class OnPointerUpCallback : MonoBehaviour, IPointerUpHandler
    {
        public event Action<PointerEventData> OnPointerUpEvent = data => { };
        public void OnPointerUp(PointerEventData eventData) => OnPointerUpEvent(eventData);
    }
}