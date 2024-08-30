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
    }
}