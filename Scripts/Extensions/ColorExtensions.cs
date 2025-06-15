using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class ColorExtensions
    {
        public static Color Average(this IEnumerable<Color> colors)
        {
            var r = 0d;
            var g = 0d;
            var b = 0d;
            var a = 0d;
            var count = 0;

            foreach (var color in colors)
            {
                r += color.r;
                g += color.g;
                b += color.b;
                a += color.a;
                count++;
            }

            return new Color(
                (float)(r / count),
                (float)(g / count),
                (float)(b / count),
                (float)(a / count)
            );
        }
    }
}