using System;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class ColliderExtensions
    {
        public static Collider[] OverlapBox(this BoxCollider boxCollider, Vector3? offset, int layermask = -1)
        {
            var off = offset ?? Vector3.zero;
            var worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            var worldExtents = boxCollider.transform.lossyScale.Multiply(boxCollider.size) / 2 + off;
            var orientation = boxCollider.transform.rotation;

            return Physics.OverlapBox(worldCenter, worldExtents, orientation, layermask);
        }

        public static Collider[] OverlapCapsule(this CapsuleCollider capsuleCollider)
        {
#if UNITY_2020_1_OR_NEWER
            var axis = capsuleCollider.direction switch
            {
                0 => Vector3.right,
                1 => Vector3.up,
                2 => Vector3.forward,
                _ => Vector3.zero
            };

            var worldTop = capsuleCollider.transform.TransformPoint(capsuleCollider.center + axis * capsuleCollider.height/2);
            var worldBottom = capsuleCollider.transform.TransformPoint(capsuleCollider.center - axis * capsuleCollider.height/2);
            var scale = capsuleCollider.transform.lossyScale.Min();

            return Physics.OverlapCapsule(worldTop, worldBottom, capsuleCollider.radius * scale);
#else
            throw new NotSupportedException();
#endif
        }

        public static Collider[] OverlapSphere(this SphereCollider sphereCollider)
        {
            var scale = sphereCollider.transform.lossyScale.Min();
            var worldCenter = sphereCollider.transform.TransformPoint(sphereCollider.center);
            var worldRadius = sphereCollider.radius * scale;
            
            return Physics.OverlapSphere(worldCenter, worldRadius);
        }
        
        public static Collider[] Overlap(this Collider collider)
        {
            switch (collider)
            {
                case SphereCollider sc: return sc.OverlapSphere(); 
                case BoxCollider bc: return bc.OverlapBox(null); 
                case CapsuleCollider cc: return cc.OverlapCapsule(); 
            }
            
            throw new ArgumentException($"Collider of type: {collider.GetType().GetSimpleAliasName()} is not supported.");
        }

        public static float Volume(this BoxCollider c)
        {
            return c.size.x * c.size.y * c.size.z;
        }
        
        public static float Volume(this SphereCollider c)
        {
            return 4 / 3f * Mathf.PI * c.radius * c.radius * c.radius;
        }
        
        public static float Volume(this MeshCollider c)
        {
            return c.sharedMesh.Volume();
        }
        
        public static float Volume(this Collider collider)
        {
            switch (collider)
            {
                case SphereCollider sc: return sc.Volume(); 
                case BoxCollider bc: return bc.Volume(); 
                case MeshCollider mc: return mc.Volume(); 
            }
            
            throw new ArgumentException($"Collider of type: {collider.GetType().GetSimpleAliasName()} is not supported.");
        }
    }
}