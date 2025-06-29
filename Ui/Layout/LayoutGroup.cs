using System;
using System.Linq;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
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

        private (ILayoutComponent layout, Transform transform)[] layoutChildren;
        private Rect rect;

        private void Awake()
        {
            layoutChildren = transform.Children()
                .Select(c => (layout: c.GetComponent<ILayoutComponent>(), c.transform))
                .Where(l => l.layout != null)
                .ToArray();

            // Hook up
            for (var i = 0; i < layoutChildren.Length; i++)
            {
                var child = layoutChildren[i];
                child.layout.OnRectChanged += () => Refresh();
                child.transform.gameObject.GetOrAddComponent<ShowLayoutBounds>().LayoutComponent = child.layout;
            }

            Refresh();
        }

        private void Refresh()
        {
            if (layoutChildren == null || layoutChildren.Length == 0)
                return;
            
            var minx = float.PositiveInfinity;
            var miny = float.PositiveInfinity;
            var maxx = float.NegativeInfinity;
            var maxy = float.NegativeInfinity;

            var t = 0f;

            // natural arrangement (down) is reversed
            var flipOrder = !reverseArrangement;

            var indexes = Enumerable.Range(0, layoutChildren.Length).ToArray();
            if (flipOrder)
            {
                indexes.ReverseInPlace();
            }

            for (var i = 0; i < indexes.Length; i++)
            {
                var index = indexes[i];
                var layoutChild = layoutChildren[index];
                var childRect = layoutChild.layout.Rect;
                var childSize = childRect.size;

                var childAlignDelta = childAlign.GetAlignDelta();

                var isLast = index == indexes[layoutChildren.Length - 1];
                var currentOffset = isLast ? 0 : spacing;

                switch (orientation)
                {
                    case LayoutGroupOrientation.Horizontal:
                        var hx = t - childRect.min.x;
                        var hy = -childRect.min.y - childAlignDelta * childSize.y;
                        layoutChild.transform.localPosition = new Vector3(hx, hy, 0);
                        t += childSize.x + currentOffset;
                        break;

                    case LayoutGroupOrientation.Vertical:
                        var vx = -childRect.min.x - childAlignDelta * childSize.x;
                        var vy = t - childRect.min.y;
                        layoutChild.transform.localPosition = new Vector3(vx, vy, 0);
                        t += childSize.y + currentOffset;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var localPos = layoutChild.transform.localPosition;
                minx = Mathf.Min(minx, localPos.x + childRect.xMin);
                miny = Mathf.Min(miny, localPos.y + childRect.yMin);
                maxx = Mathf.Max(maxx, localPos.x + childRect.xMax);
                maxy = Mathf.Max(maxy, localPos.y + childRect.yMax);
            }

            rect = Rect.MinMaxRect(minx,miny, maxx, maxy);

            OnRectChanged?.Invoke();
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