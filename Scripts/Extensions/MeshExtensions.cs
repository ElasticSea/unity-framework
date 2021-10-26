using UnityEngine;

namespace ElasticSea.Framework.Extensions
{
    public static class MeshExtensions
    {
        public static float Volume(this Mesh mesh)
        {
            float volume = 0;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                var p1 = vertices[triangles[i + 0]];
                var p2 = vertices[triangles[i + 1]];
                var p3 = vertices[triangles[i + 2]];
                volume += SignedVolumeOfTriangle(p1, p2, p3);
            }

            return Mathf.Abs(volume);
        }

        private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var v321 = p3.x * p2.y * p1.z;
            var v231 = p2.x * p3.y * p1.z;
            var v312 = p3.x * p1.y * p2.z;
            var v132 = p1.x * p3.y * p2.z;
            var v213 = p2.x * p1.y * p3.z;
            var v123 = p1.x * p2.y * p3.z;
            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }
        
        /// <summary>
        /// Moves vertices from the center to each direction
        /// </summary>
        public static Mesh OffsetMeshFromCenter(this Mesh mesh, Vector3 size)
        {
            var bounds = mesh.bounds;
            var vertices = mesh.vertices;
            var offset = (size - bounds.size) / 2;
            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = vertices[i];
                var x = vert.x > bounds.center.x / 2 ? vert.x + offset.x : vert.x - offset.x;
                var y = vert.y > bounds.center.y / 2 ? vert.y + offset.y : vert.y - offset.y;
                var z = vert.z > bounds.center.z / 2 ? vert.z + offset.z : vert.z - offset.z;
                vertices[i] = new Vector3(x, y, z);
            }
        
            var newMesh = new Mesh
            {
                vertices = vertices,
                triangles = mesh.triangles,
                normals = mesh.normals,
                uv = mesh.uv,
                tangents = mesh.tangents
            };
            newMesh.RecalculateBounds();
            return newMesh;
        }
    }
}