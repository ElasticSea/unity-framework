using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnPointerExitCallback : MonoBehaviour, IPointerExitHandler
    {
        public event Action<PointerEventData> OnPointerExitEvent = data => { };
        public void OnPointerExit(PointerEventData eventData) => OnPointerExitEvent(eventData);
    }
}