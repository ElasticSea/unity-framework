using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class Texture2DExtensions
    {
        public static Color AverageColor(this Texture2D texture)
        {
            var colors = texture.GetPixels32();
            var r = 0f;
            var g = 0f;
            var b = 0f;
            var a = 0f;
            var length = colors.Length;
            
            for (var i = 0; i < length; i++)
            {
                var c = colors[i];
                r += c.r / 255f;
                g += c.g / 255f;
                b += c.b / 255f;
                a += c.a / 255f;
            }

            return new Color(r / length, g / length, b / length, a / length);
        }
    }
}