using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Util.Callbacks
{
    public class OnPointerClickCallback : MonoBehaviour, IPointerClickHandler
    {
        public event Action<PointerEventData> OnPointerClickEvent = data => { };
        public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent(eventData);
    }
}