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
            Handles.matrix = Gizmos.matrix;
            Handles.DrawBezier(from, to, from, to, Gizmos.color, null, thickness);
#endif
        }

        public static void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
        {
#if UNITY_EDITOR
            // Special case when both points are in the same position
            if (p1 == p2)
            {
                // DrawWireSphere works only in gizmo methods
                Gizmos.DrawWireSphere(p1, radius);
                return;
            }

            using (new Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
            {
                Quaternion p1Rotation = Quaternion.LookRotation(p1 - p2);
                Quaternion p2Rotation = Quaternion.LookRotation(p2 - p1);
                // Check if capsule direction is collinear to Vector.up
                float c = Vector3.Dot((p1 - p2).normalized, Vector3.up);
                if (c == 1f || c == -1f)
                {
                    // Fix rotation
                    p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
                }

                // First side
                Handles.DrawWireArc(p1, p1Rotation * Vector3.left, p1Rotation * Vector3.down, 180f, radius);
                Handles.DrawWireArc(p1, p1Rotation * Vector3.up, p1Rotation * Vector3.left, 180f, radius);
                Handles.DrawWireDisc(p1, (p2 - p1).normalized, radius);
                // Second side
                Handles.DrawWireArc(p2, p2Rotation * Vector3.left, p2Rotation * Vector3.down, 180f, radius);
                Handles.DrawWireArc(p2, p2Rotation * Vector3.up, p2Rotation * Vector3.left, 180f, radius);
                Handles.DrawWireDisc(p2, (p1 - p2).normalized, radius);
                // Lines
                Handles.DrawLine(p1 + p1Rotation * Vector3.down * radius, p2 + p2Rotation * Vector3.down * radius);
                Handles.DrawLine(p1 + p1Rotation * Vector3.left * radius, p2 + p2Rotation * Vector3.right * radius);
                Handles.DrawLine(p1 + p1Rotation * Vector3.up * radius, p2 + p2Rotation * Vector3.up * radius);
                Handles.DrawLine(p1 + p1Rotation * Vector3.right * radius, p2 + p2Rotation * Vector3.left * radius);
            }
#endif
        }

        public static void DrawLabel(Vector3 position, string text, GUIStyle style = null)
        {
#if UNITY_EDITOR
            if (style == null)
            {
                Handles.Label(position, text);
            }
            else
            {
                Handles.Label(position, text, style);
            }
#endif
        }

        public static void DrawCircle(Vector3 pos, Vector3 normal, float radius)
        {
            // 1 segment per cm
            var circumference = 2 * Mathf.PI * radius;
            var segments = Mathf.CeilToInt(circumference / 0.01f);
            DrawCircle(pos, normal, radius, segments);
        }

        public static void DrawCircle(Vector3 pos, Vector3 normal, float radius, int numSegments)
        {
            // I t$$anonymous$$nk of normal as conceptually in the Y direction.  We find the
            // "forward" and "right" axes relative to normal and I t$$anonymous$$nk of them 
            // as the X and Z axes, though they aren't in any particular direction.
            // All that matters is that they're perpendicular to each other and on
            // the plane defined by pos and normal.
            var temp = (normal.x < normal.z) ? new Vector3(1f, 0f, 0f) : new Vector3(0f, 0f, 1f);
            var forward = Vector3.Cross(normal, temp).normalized;
            var right = Vector3.Cross(forward, normal).normalized;
 
            var prevPt = pos + (forward * radius);
            float angleStep = (Mathf.PI * 2f) / numSegments;
            for (int i = 0; i < numSegments; i++)
            {
                // Get the angle for the end of t$$anonymous$$s segment.  If it's the last segment,
                // use the angle of the first point so the last segment meets up with
                // the first point exactly (regardless of floating point imprecision).
                float angle = (i == numSegments - 1) ? 0f : (i + 1) * angleStep;
 
                // Get the segment end point in local space, i.e. pretend as if the
                // normal was (0, 1, 0), forward was (0, 0, 1), right was (1, 0, 0),
                // and pos was (0, 0, 0).
                var nextPtLocal = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;
 
                // Transform from local to world coords.  nextPtLocal's x,z are distances
                // along its axes, so we want those as the distances along our right and
                // forward axes.
                var nextPt = pos + (right * nextPtLocal.x) + (forward * nextPtLocal.z);
 
                DrawLine(prevPt, nextPt);
 
                prevPt = nextPt;
            }
        }

        public static void DrawCylinder(Vector3 start, Vector3 end, float radius, int numSegments)
        {
            // I t$$anonymous$$nk of normal as conceptually in the Y direction.  We find the
            // "forward" and "right" axes relative to normal and I t$$anonymous$$nk of them 
            // as the X and Z axes, though they aren't in any particular direction.
            // All that matters is that they're perpendicular to each other and on
            // the plane defined by pos and normal.

            var normal = end - start;
            
            var temp = (normal.x < normal.z) ? new Vector3(1f, 0f, 0f) : new Vector3(0f, 0f, 1f);
            var forward = Vector3.Cross(normal, temp).normalized;
            var right = Vector3.Cross(forward, normal).normalized;
 
            var prevPt1 = start + (forward * radius);
            var prevPt2 = end + (forward * radius);
            float angleStep = (Mathf.PI * 2f) / numSegments;
            for (int i = 0; i < numSegments; i++)
            {
                // Get the angle for the end of t$$anonymous$$s segment.  If it's the last segment,
                // use the angle of the first point so the last segment meets up with
                // the first point exactly (regardless of floating point imprecision).
                float angle = (i == numSegments - 1) ? 0f : (i + 1) * angleStep;
 
                // Get the segment end point in local space, i.e. pretend as if the
                // normal was (0, 1, 0), forward was (0, 0, 1), right was (1, 0, 0),
                // and pos was (0, 0, 0).
                var nextPtLocal = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;
 
                // Transform from local to world coords.  nextPtLocal's x,z are distances
                // along its axes, so we want those as the distances along our right and
                // forward axes.
                var nextPt1 = start + (right * nextPtLocal.x) + (forward * nextPtLocal.z);
                var nextPt2 = end + (right * nextPtLocal.x) + (forward * nextPtLocal.z);
 
                DrawLine(prevPt1, nextPt1);
                DrawLine(prevPt2, nextPt2);
                DrawLine(nextPt1, nextPt2);
 
                prevPt1 = nextPt1;
                prevPt2 = nextPt2;
            }
        }

        public static GameObject DrawDebugGameobjectSphere(Vector3 position, Color color, float scale = 0.001f)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Object.Destroy(sphere.GetComponent<Collider>());
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * scale;
            sphere.GetComponent<Renderer>().material.color = color;
            return sphere;
        }

        public static GameObject DrawDebugGameobjectCube(Vector3 position, Color color, float scale = 0.001f)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.Destroy(sphere.GetComponent<Collider>());
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * scale;
            sphere.GetComponent<Renderer>().material.color = color;
            return sphere;
        }
    }
}