using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Util
{
    public class GizmoUtils
    {
        public static void DrawPath(IEnumerable<Vector3> path) => DrawPath(path.ToList());

        public static void DrawPath(IList<Vector3> path)
        {
            if (path.Count >= 2)
            {
                for (var i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }
    }
}