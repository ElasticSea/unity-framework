using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ElasticSea.Framework.Util
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

        public static void DrawLine(Vector3 from, Vector3 to, float thickness = 1)
        {
#if UNITY_EDITOR
            Handles.DrawBezier(from, to, from, to, Gizmos.color, null, thickness);
#endif
        }
    }
}