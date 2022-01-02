using System.Collections.Generic;
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
        
        public static Mesh Scale(this Mesh mesh, Vector3 scale)
        {
            var vertices = mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].Multiply(scale);
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            return mesh;
        }
        
        public static Mesh Translate(this Mesh mesh, Vector3 translate)
        {
            var vertices = mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i] + translate;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            return mesh;
        }
        
        public static Mesh ClampMesh(this Mesh m, float clampAmount)
        {
            var bounds = m.bounds;
            bounds.Expand(-clampAmount);
            return ClampMeshToBounds(m, bounds);
        }
        
        public static Mesh ClampMeshToBounds(this Mesh m, Bounds bounds)
        {
            var vertices = m.vertices;

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].Clamp(bounds.min, bounds.max);
            }

            m.vertices = vertices;
            m.RecalculateBounds();
            return m;
        }
        
        public static void Inflate(this Mesh m, float amount)
        {
            var vertices = m.vertices;
            
            var offsets = new Vector3[vertices.Length];
            
            var groups = new Dictionary<int, List<int> >();
            var visited=  new HashSet<int>();

            for (var i = 0; i < vertices.Length; i++)
            {
                if (visited.Contains(i) == false)
                {
                    var vi = vertices[i];
                    visited.Add(i);
                    groups.Add(i, new List<int> {i});

                    for (var j = 0; j < vertices.Length; j++)
                    {
                        if (visited.Contains(j) == false)
                        {
                            var vj = vertices[j];

                            if (vi.Distance(vj) < 0.00001f)
                            {
                                groups[i].Add(j);
                                visited.Add(j);
                            }
                        }
                    }
                }
            }

            for (var s = 0; s < m.subMeshCount; s++)
            {
                var triangles = m.GetTriangles(s);
                for (var i = 0; i < triangles.Length; i += 3)
                {
                    var t0 = triangles[i + 0];
                    var t1 = triangles[i + 1];
                    var t2 = triangles[i + 2];

                    var v0 = vertices[t0];
                    var v1 = vertices[t1];
                    var v2 = vertices[t2];

                    var v10 = new Vector3(v1.x - v0.x, v1.y - v0.y, v1.z - v0.z);
                    var v20 = new Vector3(v2.x - v0.x, v2.y - v0.y, v2.z - v0.z);
                    var cross = Vector3.Cross(v10, v20);
                    var normal = cross / cross.magnitude;

                    offsets[t0] += normal;
                    offsets[t1] += normal;
                    offsets[t2] += normal;
                }
            }

            foreach (var groupIndex in groups.Keys)
            {
                var total = Vector3.zero;

                for (var vertexIndex = 0; vertexIndex < groups[groupIndex].Count; vertexIndex++)
                {
                    total += offsets[groups[groupIndex][vertexIndex]];
                }

                for (var vertexIndex = 0; vertexIndex < groups[groupIndex].Count; vertexIndex++)
                {
                    vertices[groups[groupIndex][vertexIndex]] += total.normalized * amount;
                }
            }

            m.vertices = vertices;
        }
    }
}