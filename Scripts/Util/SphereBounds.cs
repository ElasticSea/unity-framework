using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class SphereBounds
    {
        private readonly Vector3 center;
        private readonly float radius;

        public SphereBounds(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool Contains(Vector3 point)
        {
            return point.Distance(center) <= radius;
        }
    }
}