using System;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    [Serializable]
    public struct CylinderBounds
    {
        public float Height;
        public float Radius;
        public Vector3 Center;

        public CylinderBounds(Vector3 center, float radius, float height)
        {
            Height = height;
            Radius = radius;
            Center = center;
        }

        public Vector3 BottomCenter()
        {
            return Center - new Vector3(0, Height / 2, 0);
        }

        public Vector3 TopCenter()
        {
            return Center + new Vector3(0, Height / 2, 0);
        }
        
        public static bool operator ==(CylinderBounds c1, CylinderBounds c2) 
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(CylinderBounds c1, CylinderBounds c2) 
        {
            return !c1.Equals(c2);
        }
    }
}