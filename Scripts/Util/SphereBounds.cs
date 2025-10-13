using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public struct SphereBounds
    {
        public readonly Vector3 center;
        public readonly float radius;

        public SphereBounds(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool Contains(Vector3 point)
        {
            return point.Distance(center) <= radius;
        }

        public Vector3 Center => center;

        public float Radius => radius;
        
        public static bool operator ==(SphereBounds a, SphereBounds b)
        {
            return a.center == b.center && Mathf.Approximately(a.radius, b.radius);
        }

        public static bool operator !=(SphereBounds a, SphereBounds b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is SphereBounds other && this == other;
        }

        public override int GetHashCode()
        {
            return center.GetHashCode() ^ radius.GetHashCode();
        }

        public void Deconstruct(out Vector3 center, out float radius)
        {
            center = this.center;
            radius = this.radius;
        }
    }
}