using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class SphereColliderData : ColliderData
    {
        public Vector3 Center;
        public float Radius;

        public SphereColliderData(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}