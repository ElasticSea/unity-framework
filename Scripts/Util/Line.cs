using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public struct Line
    {
        private readonly Vector3 start;
        private readonly Vector3 direction;
        private readonly float length;

        public Line(Ray ray, float length) : this(ray.origin, ray.direction, length)
        {
        }
        
        public Line(Vector3 start, Vector3 end) : this(start, (end-start).normalized, (end-start).magnitude)
        {
        }
        
        public Line(Vector3 start, Vector3 direction, float length)
        {
            this.start = start;
            this.direction = direction;
            this.length = length;
        }

        public Vector3 Start => start;
        public Vector3 End => start + direction * length;
    }
}