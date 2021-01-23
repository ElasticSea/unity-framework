using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ElasticSea.Framework.Util.Callbacks
{
    public class OnDragCallback : MonoBehaviour, IDragHandler
    {
        public event Action<PointerEventData> OnDragEvent = data => { };
        public void OnDrag(PointerEventData eventData) => OnDragEvent(eventData);
    }
}