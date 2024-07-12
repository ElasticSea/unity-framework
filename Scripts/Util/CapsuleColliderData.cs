using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class CapsuleColliderData : ColliderData
    {
        public Vector3 Start;
        public Vector3 End;
        public float Radius;

        public CapsuleColliderData(Vector3 start, Vector3 end, float radius)
        {
            Start = start;
            End = end;
            Radius = radius;
        }
    }
}