using UnityEngine;

namespace ElasticSea.Framework.Util.Gizmo
{
    public interface IGizmoProvider
    {
        public Matrix4x4 Matrix { set; }
        public Color Color { set; }
        public void DrawLine(Vector3 from, Vector3 to);
        public void DrawRay(Vector3 from, Vector3 direction);
        public void DrawWireCapsule(Vector3 from, Vector3 to, float radius);
        public void DrawWireCircle(Vector3 center, Vector3 normal, float radius);
        public void DrawWireSphere(Vector3 center, float radius);
        public void DrawWireCube(Vector3 center, Vector3 size);
    }
}