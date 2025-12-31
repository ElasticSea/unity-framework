using System;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public struct CircleBounds : IEquatable<CircleBounds>
    {
        public Vector2 Center;
        public float Radius;

        public CircleBounds(Vector2 center, float radius)
        {
            Radius = radius;
            Center = center;
        }
        
        public static bool operator ==(CircleBounds c1, CircleBounds c2) 
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(CircleBounds c1, CircleBounds c2) 
        {
            return !c1.Equals(c2);
        }

        public bool Equals(CircleBounds other)
        {
            return Center.Equals(other.Center) && Radius.Equals(other.Radius);
        }

        public override bool Equals(object obj)
        {
            return obj is CircleBounds other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Center, Radius);
        }
    }
}