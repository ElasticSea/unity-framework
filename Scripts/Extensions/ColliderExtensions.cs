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
    }
}