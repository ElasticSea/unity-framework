using UnityEngine;

namespace ElasticSea.Framework.Scripts.Extensions
{
    public static class RigidbodyExtensions
    {
        public static Vector3 WorldCenterOfMass(this Rigidbody rigidbody)
        {
            return rigidbody.transform.TransformPoint(rigidbody.centerOfMass);
        }
    }
}