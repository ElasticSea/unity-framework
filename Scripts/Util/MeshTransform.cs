using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class MeshTransform
    {
        private Mesh mesh;
        
        private readonly Vector3[] vertices;
        private readonly Vector3[] normals;
        private readonly Bounds bounds;

        public MeshTransform(Mesh mesh)
        {
            this.mesh = mesh;

            vertices = mesh.vertices;
            normals = mesh.normals;
            bounds = mesh.bounds;
        }

        public MeshTransform Scale(float scale)
        {
            return Scale(new Vector3(scale, scale, scale));
        }

        public MeshTransform Scale(Vector3 scale)
        {
            var length = vertices.Length;
            for (int i = 0; i < length; i++)
            {
                vertices[i] = Vector3.Scale(vertices[i], scale);
            }

            return this;
        }

        public MeshTransform Translate(Vector3 translate)
        {
            var length = vertices.Length;
            for (int i = 0; i < length; i++)
            {
                vertices[i] += translate;
            }

            return this;
        }

        public MeshTransform RotateAround(Vector3 center, Vector3 axis, float angle)
        {
            var length = vertices.Length;
            for (int i = 0; i < length; i++)
            {
                var vec = vertices[i] - center;
                var rotation = Quaternion.AngleAxis(angle, axis);
                vertices[i] = rotation * vec + center;
                normals[i] = rotation * normals[i];
            }

            return this;
        }

        public MeshTransform RotateAround(Vector3 axis, float angle)
        {
            return RotateAround(bounds.center, axis, angle);
        }

        public Mesh GetMesh()
        {
            var cloned = mesh.Clone();
            cloned.vertices = vertices;
            cloned.normals = normals;
            cloned.RecalculateBounds();
            return cloned;
        }
    }
}