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
        [SerializeField] private Align selfHorizontalAlign = Align.Center;
        [SerializeField] private Align selfVerticalAlign = Align.Center;
        [SerializeField] private bool reverseArrangement;
        [SerializeField] private float spacing;
        [SerializeField] private float paddingLeft;
        [SerializeField] private float paddingRight;
        [SerializeField] private float paddingTop;
        [SerializeField] private float paddingBottom;

        private Dictionary<ILayoutComponent, bool> elementsMap = new ();
        private Rect rect;
        public Rect Rect => rect;
        public IEnumerable<ILayoutComponent> Elements => elementsMap.Keys.ToArray();
        public event Action OnRectChanged;

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
            var elementsLength = GetOrderedElements();

            if (elementsLength > 0)
            {
                var minx = float.PositiveInfinity;
                var miny = float.PositiveInfinity;
                var maxx = float.NegativeInfinity;
                var maxy = float.NegativeInfinity;

                var t = 0f;
                
                // Calculate Rect
                var childOffsets = new Vector2[elementsLength];
                for (var i = 0; i < elementsLength; i++)
                {
                    var element = buffer[i];
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
                            childOffsets[i]= new Vector2(hx, hy);
                            t += childSize.x + currentOffset;
                            break;

                        case LayoutGroupOrientation.Vertical:
                            var vx = -childRect.min.x - childAlignDelta * childSize.x;
                            var vy = t - childRect.min.y;
                            childOffsets[i] = new Vector2(vx, vy);
                            t += childSize.y + currentOffset;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var localPos = childOffsets[i];
                    minx = Mathf.Min(minx, localPos.x + childRect.xMin - paddingLeft);
                    miny = Mathf.Min(miny, localPos.y + childRect.yMin - paddingBottom);
                    maxx = Mathf.Max(maxx, localPos.x + childRect.xMax + paddingRight);
                    maxy = Mathf.Max(maxy, localPos.y + childRect.yMax + paddingTop);
                }
                
                rect = Rect.MinMaxRect(minx, miny, maxx, maxy);

                // Apply to children
                var innerXOffset = selfHorizontalAlign.GetAlignDelta() * rect.width - rect.max.x;
                var innerYOffset = selfVerticalAlign.GetAlignDelta() * rect.height - rect.max.y;
                var interRectOffset = new Vector2(innerXOffset, innerYOffset);
                rect.position += interRectOffset;
                for (var i = 0; i < elementsLength; i++)
                {
                    var element = buffer[i];
                    var offset = childOffsets[i];
                    var elementTransform = ((Component)element).transform;
                    elementTransform.localPosition = offset  + interRectOffset;
                }
            }
            else
            {
                rect = Rect.zero;
            }

            OnRectChanged?.Invoke();
        }

        private int GetOrderedElements()
        {
            var flipOrder = reverseArrangement;
            if (orientation == LayoutGroupOrientation.Vertical)
            {
                // natural arrangement (down) is reversed in vertical
                flipOrder = !reverseArrangement;
            }
            
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

        public void Clear()
        {
            elementsMap.Clear();
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
    }
}