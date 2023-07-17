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
        
        public static int OverlapBoxNonAlloc(this BoxCollider boxCollider, Collider[] results, int layermask = -1, float offset = 0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
            var worldExtents = boxCollider.transform.lossyScale.Multiply(boxCollider.size) / 2 + new Vector3(offset, offset, offset);
            var orientation = boxCollider.transform.rotation;

            return Physics.OverlapBoxNonAlloc(worldCenter, worldExtents, results, orientation, layermask, queryTriggerInteraction);
        }

        public static Collider[] OverlapCapsule(this CapsuleCollider capsuleCollider, int layermask = -1)
        {
            var (start, end) = capsuleCollider.GetCapsulePoints();

            var worldBottom = capsuleCollider.transform.TransformPoint(start);
            var worldTop = capsuleCollider.transform.TransformPoint(end);
            var scale = capsuleCollider.transform.lossyScale.Min();

            return Physics.OverlapCapsule(worldTop, worldBottom, capsuleCollider.radius * scale, layermask);
        }

        public static (Vector3 start, Vector3 end) GetCapsulePoints(this CapsuleCollider capsuleCollider)
        {
            Vector3 axis;
            switch (capsuleCollider.direction)
            {
                case 0:
                    axis = Vector3.right;
                    break;
                case 1:
                    axis = Vector3.up;
                    break;
                case 2:
                    axis = Vector3.forward;
                    break;
                default:
                    axis = Vector3.zero;
                    break;
            }

            var betweenPointsHeight = capsuleCollider.height - capsuleCollider.radius * 2;
            var start = capsuleCollider.center - axis * betweenPointsHeight / 2;
            var end = capsuleCollider.center + axis * betweenPointsHeight / 2;
            return (start, end);
        }

        public static int OverlapCapsuleNonAlloc(this CapsuleCollider capsuleCollider, Collider[] results, int layermask = -1, float offset = 0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var (start, end) = capsuleCollider.GetCapsulePoints();

            var worldBottom = capsuleCollider.transform.TransformPoint(start);
            var worldTop = capsuleCollider.transform.TransformPoint(end);
            var scale = capsuleCollider.transform.lossyScale.Min();

            return Physics.OverlapCapsuleNonAlloc(worldTop, worldBottom, capsuleCollider.radius * scale + offset, results, layermask, queryTriggerInteraction);
        }

        public static Collider[] OverlapSphere(this SphereCollider sphereCollider, int layermask = -1)
        {
            var scale = sphereCollider.transform.lossyScale.Min();
            var worldCenter = sphereCollider.transform.TransformPoint(sphereCollider.center);
            var worldRadius = sphereCollider.radius * scale;
            
            return Physics.OverlapSphere(worldCenter, worldRadius, layermask);
        }

        public static int OverlapSphereNonAlloc(this SphereCollider sphereCollider, Collider[] results, int layermask = -1, float offset = 0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            var scale = sphereCollider.transform.lossyScale.Min();
            var worldCenter = sphereCollider.transform.TransformPoint(sphereCollider.center);
            var worldRadius = sphereCollider.radius * scale;
            
            return Physics.OverlapSphereNonAlloc(worldCenter, worldRadius + offset, results, layermask, queryTriggerInteraction);
        }
        
        public static Collider[] Overlap(this Collider collider, int layermask = -1)
        {
            switch (collider)
            {
                case SphereCollider sc: return sc.OverlapSphere(layermask); 
                case BoxCollider bc: return bc.OverlapBox(null, layermask); 
                case CapsuleCollider cc: return cc.OverlapCapsule(layermask); 
            }
            
            throw new ArgumentException($"Collider of type: {collider.GetType().GetSimpleAliasName()} is not supported.");
        }
        
        public static int OverlapNonAlloc(this Collider collider, Collider[] results, int layermask = -1, float offset = 0, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            switch (collider)
            {
                case SphereCollider sc: return sc.OverlapSphereNonAlloc(results, layermask, offset, queryTriggerInteraction);
                case BoxCollider bc: return bc.OverlapBoxNonAlloc(results, layermask, offset, queryTriggerInteraction); 
                case CapsuleCollider cc: return cc.OverlapCapsuleNonAlloc(results, layermask, offset, queryTriggerInteraction);
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
        
        public static float Volume(this CapsuleCollider c)
        {
            // πr2((4/3)r + a);
            var r = c.radius;
            var a = c.height - r * 2;
            var pi = Mathf.PI;
            return pi * r * r * ((4f / 3f) * r + a);
        }
        
        public static float Volume(this Collider collider)
        {
            switch (collider)
            {
                case SphereCollider sc: return sc.Volume(); 
                case BoxCollider bc: return bc.Volume(); 
                case MeshCollider mc: return mc.Volume(); 
                case CapsuleCollider cc: return cc.Volume(); 
            }
            
            throw new ArgumentException($"Collider of type: {collider.GetType().GetSimpleAliasName()} is not supported.");
        }
        
        public static bool IsColliding(this Collider thisCollider, Collider[] otherColliders)
        {
            var length = otherColliders.Length;
            for (var i = 0; i < length; i++)
            {
                var otherCollider = otherColliders[i];
                var otherTransform = otherCollider.transform;
                var thisTransform = thisCollider.transform;
                if (Physics.ComputePenetration(
                        otherCollider,
                        otherTransform.position,
                        otherTransform.rotation,
                        thisCollider,
                        thisTransform.position,
                        thisTransform.rotation,
                        out var direction,
                        out var distance
                    ))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static Bounds LocalBounds(this BoxCollider boxCollider)
        {
            return new Bounds(boxCollider.center, boxCollider.size);
        }
    }
}