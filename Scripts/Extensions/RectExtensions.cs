using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class RectExtensions
    {
        public static Rect Encapsulate(this IEnumerable<Rect> rects)
        {
            return rects.ToArray().Encapsulate();
        }
        
        public static Rect Encapsulate(this Rect rect, Rect other)
        {
            var final = new Rect();
            final.min = rect.min.Min(other.min);
            final.max = rect.max.Max(other.max);
            return final;
        }
        
        public static Rect Encapsulate(this Rect[] rects)
        {
            var length = rects.Length;
            if (length == 0)
            {
                return default;
            }

            var rect = rects[0];
            for (var i = 1; i < length; i++)
            {
                var otherRect = rects[i];
                rect.min = rect.min.Min(otherRect.min);
                rect.max = rect.max.Max(otherRect.max);
            }

            return rect;
        }

        public static Rect Grow(this Rect rect, float growBy)
        {
            return rect.Grow(new Vector2(growBy, growBy));
        }

        public static Rect Grow(this Rect rect, Vector2 growBy)
        {
            return Rect.MinMaxRect(rect.xMin - growBy.x, rect.yMin - growBy.y, rect.xMax + growBy.x, rect.yMax + growBy.y);
        }

        public static Rect Shrink(this Rect rect, float shrinkBy)
        {
            return rect.Grow(-shrinkBy);
        }

        public static Rect Shrink(this Rect rect, Vector2 shrinkBy)
        {
            return rect.Grow(-shrinkBy);
        }

        public static Rect Move(this Rect rect, Vector2 moveBy)
        {
            var newRect = new Rect(rect);
            newRect.position += moveBy;
            return newRect;
        }

        public static (Rect left, Rect right) SplitHorizontal(this Rect rect)
        {
            var centerPoint = Mathf.Lerp(rect.xMin, rect.xMax, 0.5f);
            var left = Rect.MinMaxRect(rect.xMin, rect.yMin, centerPoint, rect.yMax);
            var right = Rect.MinMaxRect(centerPoint, rect.yMin, rect.xMax, rect.yMax);
            return (left, right);
        }

        public static (Rect bottom, Rect top) SplitVertical(this Rect rect)
        {
            var centerPoint = Mathf.Lerp(rect.yMin, rect.yMax, 0.5f);
            var bottom = Rect.MinMaxRect(rect.xMin, rect.yMin, rect.xMax, centerPoint);
            var top = Rect.MinMaxRect(rect.xMin, centerPoint, rect.xMax, rect.yMax);
            return (bottom, top);
        }

        public static Vector2[] Vertices(this Rect rect)
        {
            return new Vector2[]
            {
                new (rect.xMin, rect.yMin),
                new (rect.xMin, rect.yMax),
                new (rect.xMax, rect.yMax),
                new (rect.xMax, rect.yMin),
            };
        }
    }
}