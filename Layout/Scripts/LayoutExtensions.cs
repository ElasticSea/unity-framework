using System;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public static class LayoutExtensions
    {
        public static Vector2 AlignInsideRect(this Rect rect, Rect toAlign, Align horizontal, Align vertical, float padding = 0)
        {
            return AlignInsideRect(rect, toAlign, horizontal, vertical, new Vector2(padding, padding));
        }
        
        public static Vector2 AlignInsideRect(this Rect rect, Rect toAlign, Align horizontal, Align vertical, Vector2 padding)
        {
            var offset = new Vector2();
            offset.x = GetPosition(horizontal, rect.xMin, rect.xMax, toAlign.xMin, toAlign.xMax, padding.x);
            offset.y = GetPosition(vertical, rect.yMin, rect.yMax, toAlign.yMin, toAlign.yMax, padding.y);
            return offset;
        }

        private static float GetPosition(Align align, float rectMin, float rectMax, float toAlignMin, float toAlignMax, float padding)
        {
            return align switch
            {
                Align.BeforeStart => rectMin - toAlignMax - padding,
                Align.Start       => rectMin - toAlignMin + padding,
                Align.Center      => (rectMin + rectMax - toAlignMin - toAlignMax) / 2,
                Align.End         => rectMax - toAlignMax - padding,
                Align.AfterEnd    => rectMax - toAlignMin + padding,
                _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
            };
        }

        public static float GetAlignDelta(this Align align)
        {
            switch (align)
            {
                case Align.Start:
                    return 0;
                case Align.Center:
                    return 0.5f;
                case Align.End:
                    return 1f;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void Bind(this LayoutGroup element, Action<Rect> onRectChanged)
        {
            element.OnRectChanged += () => onRectChanged(element.Rect);
            onRectChanged(element.Rect);
        }
    }
}