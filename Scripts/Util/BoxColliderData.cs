using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class BoxColliderData : ColliderData
    {
        public Vector3 Center;
        public Vector3 Size;

        public BoxColliderData(Vector3 center, Vector3 size)
        {
            Center = center;
            Size = size;
        }
    }
}