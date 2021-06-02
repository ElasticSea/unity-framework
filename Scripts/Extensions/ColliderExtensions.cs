using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class ColliderExtensions
    {
        public static Collider[] Overlap(this BoxCollider boxCollider)
        {
            var worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            var worldExtents = boxCollider.transform.lossyScale.Multiply(boxCollider.size) / 2;
            var orientation = boxCollider.transform.rotation;

            return Physics.OverlapBox(worldCenter, worldExtents, orientation);
        }
        
        public static Collider[] Overlap(this SphereCollider sphereCollider)
        {
            var scale = sphereCollider.transform.lossyScale.Min();
            var worldCenter = sphereCollider.transform.TransformPoint(sphereCollider.center);
            var worldRadius = sphereCollider.radius * scale;
            
            return Physics.OverlapSphere(worldCenter, worldRadius);
        }
    }
}