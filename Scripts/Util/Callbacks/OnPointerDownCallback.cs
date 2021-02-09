using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnPointerDownCallback : MonoBehaviour, IPointerDownHandler
    {
        public event Action<PointerEventData> OnPointerDownEvent = data => { };
        public void OnPointerDown(PointerEventData eventData) => OnPointerDownEvent(eventData);
    }
}