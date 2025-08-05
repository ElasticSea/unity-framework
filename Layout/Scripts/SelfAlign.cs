using System;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class SelfAlign : MonoBehaviour
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _layout;
        [SerializeField] private Align horizontal = Align.Center;
        [SerializeField] private Align vertical = Align.Center;

        private void OnEnable()
        {
            ((ILayoutComponent)_layout).OnRectChanged += Refresh;
        }

        private void OnDisable()
        {
            ((ILayoutComponent)_layout).OnRectChanged -= Refresh;
        }

        private void Refresh()
        {
            var layout = (ILayoutComponent)_layout;
            var rect = layout.Rect;

            var offset = Vector2.zero;
            offset.x -= Mathf.Lerp(rect.xMin, rect.xMax, GetDelta(horizontal));
            offset.y -= Mathf.Lerp(rect.yMin, rect.yMax, GetDelta(vertical));
            transform.localPosition = offset;
        }

        private float GetDelta(Align align)
        {
            switch (align)
            {
                case Align.BeforeStart:
                case Align.Start:
                    return 0;
                case Align.AfterEnd:
                case Align.End:
                    return 1f;
                case Align.Center:
                    return 0.5f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(align), align, null);
            }
        }
    }
}