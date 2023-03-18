using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class PhysicsUtils
    {
        public static bool BoxTriangleIntersect(Bounds box, Vector3 t0, Vector3 t1, Vector3 t2)
        {
            return BoxTriangleIntersectorOld.IsIntersecting(box, t0, t1, t2);
        }
        
        public static bool BoxTriangleIntersectFast(BoxTriangleIntersector.FastBox box, BoxTriangleIntersector.FastTriangle triangle)
        {
            return BoxTriangleIntersector.IsIntersecting(box, triangle);
        }
    }
}