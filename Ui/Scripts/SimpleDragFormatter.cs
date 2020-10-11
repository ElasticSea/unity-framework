using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class SimpleDragFormatter : OnDragFormatter
    {
        [SerializeField] private Image highlight;
        [SerializeField] private GameObject before;
        [SerializeField] private GameObject middle;
        [SerializeField] private GameObject after;
        
        private GameObject[] objects;

        private void Awake()
        {
            objects = new[] {before, middle, after};
            OnDragChanged(DragPosition.None);
            OnOriginChanged(false);
        }

        public override void OnDragChanged(DragPosition dragPosition)
        {
            switch (dragPosition)
            {
                case DragPosition.None:
                    Select(null);
                    break;
                case DragPosition.Before:
                    Select(before);
                    break;
                case DragPosition.Middle:
                    Select(middle);
                    break;
                case DragPosition.After:
                    Select(after);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dragPosition), dragPosition, null);
            }
        }

        public override void OnOriginChanged(bool isOrigin)
        {
            highlight.enabled = isOrigin;
        }

        private void Select(GameObject selectThis)
        {
            foreach (var obj in objects)
            {
                obj.SetActive(obj == selectThis);
            }
        }
    }
}