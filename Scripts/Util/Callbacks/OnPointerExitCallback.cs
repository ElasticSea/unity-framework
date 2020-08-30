using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Framework.Scripts.Util.Callbacks
{
    public class OnPointerExitCallback : MonoBehaviour, IPointerExitHandler
    {
        public event Action<PointerEventData> OnPointerExitEvent = data => { };
        public void OnPointerExit(PointerEventData eventData) => OnPointerExitEvent(eventData);
    }
}