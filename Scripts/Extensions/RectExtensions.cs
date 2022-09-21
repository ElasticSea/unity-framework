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
    }
}