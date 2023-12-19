using System.Drawing;
using UnityEngine;

namespace Blocks.Meshbakers
{
    public class FastBounds
    {
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Size;

        public FastBounds(Bounds bounds) : this(bounds.min, bounds.max)
        {
        }

        public FastBounds(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
            Size = max - min;
        }

        public Vector3[] Vertices()
        {
            return new[]
            {
                new Vector3(Min.x, Min.y, Min.z),
                new Vector3(Min.x, Min.y, Max.z),
                new Vector3(Min.x, Max.y, Min.z),
                new Vector3(Min.x, Max.y, Max.z),
                new Vector3(Max.x, Min.y, Min.z),
                new Vector3(Max.x, Min.y, Max.z),
                new Vector3(Max.x, Max.y, Min.z),
                new Vector3(Max.x, Max.y, Max.z),
            };
        }
    }
}