using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        
        public delegate Color GetPixel(int x, int y);
        public delegate Color Build(GetPixel getPixel, int x, int y);
        public static Color[] ConvertMt(this Color[] pixels, int width, int height, Build build)
        {
            var output = new Color[width * height];
            
            var chunkSize = width * height / (SystemInfo.processorCount * 8);

            var workPackages = new List<(int start, int end)>();

            var sizeBudget = output.Length;
            var i = 0;
            while (sizeBudget > 0)
            {
                var currentChunk = Mathf.Min(chunkSize, sizeBudget);

                workPackages.Add((i, i + currentChunk));
                sizeBudget -= currentChunk;
                i += currentChunk;
            }

            GetPixel getPixel = (x, y) => pixels[y * width + x];

            Parallel.ForEach(workPackages, tuple =>
            {
                var (start, end) = tuple;
                for (var j = start; j < end; j++)
                {
                    var x = j % width;
                    var y = j / width;
                    output[j] = build(getPixel, x, y);
                }
            });

            return output;
        }
    }
}