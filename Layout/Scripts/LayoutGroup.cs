using System;
using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class LayoutGroup : MonoBehaviour, ILayoutComponent
    {
        public enum LayoutGroupOrientation
        {
            Horizontal,
            Vertical
        }

        [SerializeField] private LayoutGroupOrientation orientation;
        [SerializeField] private Align childAlign = Align.Center;
        [SerializeField] private bool reverseArrangement;
        [SerializeField] private float spacing;
        [SerializeField] private float paddingLeft;
        [SerializeField] private float paddingRight;
        [SerializeField] private float paddingTop;
        [SerializeField] private float paddingBottom;

        private Dictionary<ILayoutComponent, bool> elementsMap = new ();
        private Rect rect;

        public void AddElement(ILayoutComponent element)
        {
            if (AddElementInner(element))
            {
                Refresh();
            }
        }

        public void AddElements(params ILayoutComponent[] elements)
        {
            var any = false;
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                if (AddElementInner(element))
                {
                    any = true;
                }
            }

            if (any)
            {
                Refresh();
            }
        }
        
        private bool AddElementInner(ILayoutComponent element)
        {
            if (elementsMap.TryAdd(element, true))
            {
                element.OnRectChanged += Refresh;
                return true;
            }

            return false;
        }

        public void RemoveElement(ILayoutComponent element)
        {
            if (elementsMap.Remove(element))
            {
                element.OnRectChanged -= Refresh;
                Refresh();
            }
        }

        public void SetElementVisibility(ILayoutComponent element, bool visible)
        {
            elementsMap[element] = visible;
            ((Component)element).gameObject.SetActive(visible);
            Refresh();
        }

        private ILayoutComponent[] buffer = new ILayoutComponent[0];
        
        private void Refresh()
        {
            var minx = float.PositiveInfinity;
            var miny = float.PositiveInfinity;
            var maxx = float.NegativeInfinity;
            var maxy = float.NegativeInfinity;

            var t = 0f;

            var elementsLength = GetOrderedElements();

            for (var i = 0; i < elementsLength; i++)
            {
                var element = buffer[i];
                var elementTransform = ((Component)element).transform;
                var childRect = element.Rect;
                var childSize = childRect.size;

                var childAlignDelta = childAlign.GetAlignDelta();

                var isLast = i == elementsLength - 1;
                var currentOffset = isLast ? 0 : spacing;

                switch (orientation)
                {
                    case LayoutGroupOrientation.Horizontal:
                        var hx = t - childRect.min.x;
                        var hy = -childRect.min.y - childAlignDelta * childSize.y;
                        elementTransform.localPosition = new Vector3(hx, hy, 0);
                        t += childSize.x + currentOffset;
                        break;

                    case LayoutGroupOrientation.Vertical:
                        var vx = -childRect.min.x - childAlignDelta * childSize.x;
                        var vy = t - childRect.min.y;
                        elementTransform.localPosition = new Vector3(vx, vy, 0);
                        t += childSize.y + currentOffset;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var localPos = elementTransform.localPosition;
                minx = Mathf.Min(minx, localPos.x + childRect.xMin - paddingLeft);
                miny = Mathf.Min(miny, localPos.y + childRect.yMin - paddingBottom);
                maxx = Mathf.Max(maxx, localPos.x + childRect.xMax + paddingRight);
                maxy = Mathf.Max(maxy, localPos.y + childRect.yMax + paddingTop);
            }

            rect = Rect.MinMaxRect(minx,miny, maxx, maxy);

            OnRectChanged?.Invoke();
        }

        private int GetOrderedElements()
        {
            // natural arrangement (down) is reversed
            var flipOrder = !reverseArrangement;
            
            buffer = buffer.EnsureArray(elementsMap.Count);
            var bufferLength = 0;
            foreach (var b in elementsMap)
            {
                var isVisible = b.Value;
                if (isVisible)
                {
                    buffer[bufferLength++] = b.Key;
                }
            }
            
            if (flipOrder)
            {
                buffer.ReverseInPlace(bufferLength);
            }

            return bufferLength;
        }

        private void OnValidate()
        {
            Refresh();
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(rect.center, rect.size);
        }

        public Rect Rect => rect;
        public event Action OnRectChanged;
    }
}