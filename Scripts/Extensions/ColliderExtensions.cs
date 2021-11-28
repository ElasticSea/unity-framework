using System;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class ColliderExtensions
    {
        public static Collider[] Overlap(this CapsuleCollider capsuleCollider)
        {
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
        }
        
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
        
        public static Collider[] Overlap(this Collider collider)
        {
            switch (collider)
            {
                case SphereCollider sc: return sc.Overlap(); 
                case BoxCollider bc: return bc.Overlap(); 
                case CapsuleCollider cc: return cc.Overlap(); 
            }
            
            throw new ArgumentException($"Collider of type: {collider.GetType().GetSimpleAliasName()} is not supported.");
        }
    }
}