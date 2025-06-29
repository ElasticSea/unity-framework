using System;
using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public static class LayoutExtensions
    {
        public static Vector2 AlignInsideRect(this Rect rect, Rect toAlign, Align horizontal, Align vertical)
        {
            var offset = new Vector2();
           
            offset.x = horizontal switch
            {
                Align.BeforeStart => rect.x - toAlign.x - toAlign.width,
                Align.Start => rect.x - toAlign.x,
                Align.Center => rect.x - toAlign.x + (rect.width - toAlign.width) / 2,
                Align.End => rect.xMax - toAlign.xMax,
                Align.AfterEnd => rect.xMax - toAlign.xMax + toAlign.width,
                _ => throw new ArgumentOutOfRangeException(nameof(horizontal), horizontal, null)
            };

            offset.y = vertical switch
            {
                Align.BeforeStart => rect.y - toAlign.y - toAlign.height,
                Align.Start => rect.y - toAlign.y,
                Align.Center => rect.y - toAlign.y + (rect.height - toAlign.height) / 2,
                Align.End => rect.yMax - toAlign.yMax,
                Align.AfterEnd => rect.yMax - toAlign.yMax + toAlign.height,
                _ => throw new ArgumentOutOfRangeException(nameof(vertical), vertical, null)
            };

            return offset;
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