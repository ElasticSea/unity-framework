using System;
using System.Collections.Generic;
using _Framework.Scripts.Ui.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Framework.Ui.TreeViews
{
    public class DragDropItem : MonoBehaviour,
        IBeginDragHandler,
        IDropHandler,
        IEndDragHandler,
        IDragHandler
    {
        [SerializeField] private ListViewItem item;
        private static ListViewItem dragging;
        private static OnDragFormatter formatter;
        public Action<object, object, DragPosition> OnDropEvent = (o, o1, position) => { };

        public ListViewItem Item
        {
            get => item;
            set => item = value;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = item;
            
            var dragFormatter = item.GetComponent<OnDragFormatter>();
            if (dragFormatter)
            {
                dragFormatter.OnOriginChanged(true);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (dragging)
            {
                var position = GetPosition(formatter.gameObject);
                OnDropEvent(dragging.Value, item.Value, position);
            }

            var onDragFormatter = dragging.GetComponent<OnDragFormatter>();
            if (onDragFormatter)
            {
                onDragFormatter.OnOriginChanged(false);   
            }
            dragging = null;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (formatter)
            {
                formatter.OnDragChanged(DragPosition.None);
            }

            if (dragging)
            {
                var onDragFormatter = dragging.GetComponent<OnDragFormatter>();
                if (onDragFormatter)
                {
                    onDragFormatter.OnOriginChanged(false);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (formatter)
            {
                formatter.OnDragChanged(DragPosition.None);
            }
            formatter = GetFormatter();
            if (formatter)
            {
                var position = GetPosition(formatter.gameObject);
                formatter.OnDragChanged(position);
            }
        }

        private DragPosition GetPosition(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            var localPos = rt.transform.InverseTransformPoint(Input.mousePosition);
            var ll = Mathf.InverseLerp(rt.rect.min.y, rt.rect.max.y, localPos.y);

            if (ll < 0.25f) return DragPosition.After;
            if (ll < 0.75f) return DragPosition.Middle;
            return DragPosition.Before;
        }

        private OnDragFormatter GetFormatter()
        {
            //Fetch the Raycaster from the GameObject (the Canvas)
            var m_Raycaster = GetComponentInParent<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            var m_EventSystem = GetComponent<EventSystem>();

            //Set up the new Pointer Event
            var m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponentInParent<ListView>() == item.GetComponentInParent<ListView>())
                {
                    if (result.gameObject.GetComponent<ListViewItem>())
                    {
                        return result.gameObject.GetComponent<OnDragFormatter>();
                    }
                }
            }

            return null;
        }
    }
}