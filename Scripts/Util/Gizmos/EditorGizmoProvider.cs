using UnityEngine;

namespace ElasticSea.Framework.Util.Gizmo
{
    public class EditorGizmoProvider : IGizmoProvider
    {
        public Matrix4x4 Matrix
        {
            set => Gizmos.matrix = value;
        }

        public Color Color
        {
            set => Gizmos.color = value;
        }

        public void DrawLine(Vector3 from, Vector3 to)
        {
            Gizmos.DrawLine(from, to);
        }

        public void DrawRay(Vector3 from, Vector3 direction)
        {
            Gizmos.DrawRay(from, direction);
        }

        public void DrawWireCapsule(Vector3 from, Vector3 to, float radius)
        {
            GizmoUtils.DrawWireCapsule(from, to, radius);
        }

        public void DrawWireCircle(Vector3 from, Vector3 normal, float radius)
        {
            GizmoUtils.DrawCircle(from, normal, radius);
        }

        public void DrawSphere(Vector3 center, float radius)
        {
            Gizmos.DrawSphere(center, radius);
        }

        public void DrawWireSphere(Vector3 center, float radius)
        {
            Gizmos.DrawWireSphere(center, radius);
        }

        public void DrawWireCube(Vector3 center, Vector3 size)
        {
            Gizmos.DrawWireCube(center, size);
        }
    }
}