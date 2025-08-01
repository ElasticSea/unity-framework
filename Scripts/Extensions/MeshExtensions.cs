using System;
using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Util;
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
        public static Mesh ExtendMeshFromCenter(this Mesh source, Vector3 size)
        {
            var offset = (size - source.bounds.size) / 2;
            return source.ExtendMesh(offset, offset, new Vector3(0.5f, 0.5f, 0.5f));
        }
        
        /// <summary>
        /// Moves vertices from the center to each direction
        /// </summary>
        public static Mesh ExtendMesh(this Mesh source, Vector3 minExtends, Vector3 maxExtends, Vector3 normalizedCenter)
        {
            var mesh = source.Clone();
            var bounds = mesh.bounds;
            var vertices = mesh.vertices;
            var boundsCenter = bounds.min + bounds.size.Multiply(normalizedCenter);

            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = vertices[i];
                var x = vert.x + (vert.x > boundsCenter.x / 2 ? +minExtends.x : -maxExtends.x);
                var y = vert.y + (vert.y > boundsCenter.y / 2 ? +minExtends.y : -maxExtends.y);
                var z = vert.z + (vert.z > boundsCenter.z / 2 ? +minExtends.z : -maxExtends.z);
                vertices[i] = new Vector3(x, y, z);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            return mesh;
        }
        
        /// <summary>
        /// Sets vertices from min to max value on the x axis
        /// </summary>
        public static Mesh SetXSize(this Mesh source, float min, float max)
        {
            return source.SetXSize(min, max, Vector3.one / 2f);
        }
        
        /// <summary>
        /// Sets vertices from min to max value on the x and z axis
        /// </summary>
        public static Mesh SetXZSize(this Mesh source, Vector2 min, Vector2 max)
        {
            return source.SetXZSize(min, max, Vector3.one / 2f);
        }
        
        /// <summary>
        /// Sets vertices from min to max value on the x and z axis
        /// </summary>
        public static Mesh SetSize(this Mesh source, Vector3 min, Vector3 max)
        {
            return source.SetSize(min, max, Vector3.one / 2f);
        }
        
        /// <summary>
        /// Sets vertices from min to max value on the x axis
        /// </summary>
        public static Mesh SetXSize(this Mesh source, float min, float max, Vector3 normalizedCenter)
        {
            var mesh = source.Clone();
            var bounds = mesh.bounds;
            var vertices = mesh.vertices;
            var boundsCenterX = (bounds.min + bounds.size.Multiply(normalizedCenter)).x;
            // var originalLeftSize = boundsCenterX - bounds.min.x;
            // var originalRightSize =  bounds.max.x - boundsCenterX;

            var leftOffset = min - bounds.min.x;
            var rightOffset = max - bounds.max.x;

            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = vertices[i];
                var x = vert.x < boundsCenterX ? vert.x + leftOffset : vert.x + rightOffset;
                vertices[i] = new Vector3(x, vert.y, vert.z);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            return mesh;
        }
        
        /// <summary>
        /// Sets vertices from min to max value on the x axis
        /// </summary>
        public static Mesh SetXZSize(this Mesh source, Vector2 min, Vector2 max, Vector3 normalizedCenter)
        {
            var mesh = source.Clone();
            var bounds = mesh.bounds;
            var vertices = mesh.vertices;
            var boundsCenter = (bounds.min + bounds.size.Multiply(normalizedCenter));
            // var originalLeftSize = boundsCenterX - bounds.min.x;
            // var originalRightSize =  bounds.max.x - boundsCenterX;

            var leftOffsetX = min.x - bounds.min.x;
            var rightOffsetX = max.x - bounds.max.x;

            var leftOffsetZ = min.y - bounds.min.z;
            var rightOffsetZ = max.y - bounds.max.z;

            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = vertices[i];
                var x = vert.x < boundsCenter.x ? vert.x + leftOffsetX : vert.x + rightOffsetX;
                var z = vert.z < boundsCenter.z ? vert.z + leftOffsetZ : vert.z + rightOffsetZ;
                vertices[i] = new Vector3(x, vert.y, z);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            return mesh;
        }
        
        /// <summary>
        /// Sets vertices from min to max value on all axises
        /// </summary>
        public static Mesh SetSize(this Mesh source, Vector3 min, Vector3 max, Vector3 normalizedCenter)
        {
            var mesh = source.Clone();
            var bounds = mesh.bounds;
            var vertices = mesh.vertices;
            var boundsCenter = (bounds.min + bounds.size.Multiply(normalizedCenter));

            var leftOffsetX = min.x - bounds.min.x;
            var rightOffsetX = max.x - bounds.max.x;

            var leftOffsetY = min.y - bounds.min.y;
            var rightOffsetY = max.y - bounds.max.y;

            var leftOffsetZ = min.z - bounds.min.z;
            var rightOffsetZ = max.z - bounds.max.z;

            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = vertices[i];
                var x = vert.x < boundsCenter.x ? vert.x + leftOffsetX : vert.x + rightOffsetX;
                var y = vert.y < boundsCenter.y ? vert.y + leftOffsetY : vert.y + rightOffsetY;
                var z = vert.z < boundsCenter.z ? vert.z + leftOffsetZ : vert.z + rightOffsetZ;
                vertices[i] = new Vector3(x, y, z);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            return mesh;
        }
        
        /// <summary>
        /// Moves vertices from the center to each direction
        /// </summary>
        public static Mesh ExtendMeshToBoundsClone(this Mesh source, Bounds boundsToMatch)
        {
            var mesh = source.Clone();
            mesh.ExtendMeshToBounds(boundsToMatch);
            return mesh; 
        }
        
        /// <summary>
        /// Moves vertices from the center to each direction
        /// </summary>
        public static void ExtendMeshToBounds(this Mesh mesh, Bounds boundsToMatch)
        {
            var bounds = mesh.bounds;
            var vertices = mesh.vertices;

            ExtendMeshToBounds(vertices, vertices, bounds, boundsToMatch);

            mesh.vertices = vertices;
            mesh.bounds = boundsToMatch;
            
        }
        
        
        /// <summary>
        /// Moves vertices from the center to each direction
        /// </summary>
        public static void ExtendMeshToBounds(this Vector3[] sourceVertices, Vector3[] targetVertices, Bounds bounds, Bounds boundsToMatch)
        {
            var boundsCenter = bounds.center;

            var minExtends = boundsToMatch.min - bounds.min + boundsCenter - bounds.center;
            var maxExtends = boundsToMatch.max - bounds.max + boundsCenter - bounds.center;

            for (var i = 0; i < sourceVertices.Length; i++)
            {
                var vert = sourceVertices[i];
                var x = vert.x + (vert.x > boundsCenter.x ? maxExtends.x : minExtends.x);
                var y = vert.y + (vert.y > boundsCenter.y ? maxExtends.y : minExtends.y);
                var z = vert.z + (vert.z > boundsCenter.z ? maxExtends.z : minExtends.z);
                targetVertices[i] = new Vector3(x, y, z) ;
            }
        }
        
        public static Mesh Clone(this Mesh mesh)
        {
            var newMesh = new Mesh
            {
                indexFormat = mesh.indexFormat,
                name = mesh.name,
                vertices = mesh.vertices,
                normals = mesh.normals,
                tangents = mesh.tangents,
                subMeshCount = mesh.subMeshCount,
                bounds = mesh.bounds
            };

            // Copy uvs this way, otherwise the uvs are implicitly converted to Vector2
            for (var i = 0; i < 8; i++)
            {
                var uvs = new List<Vector4>();
                mesh.GetUVs(i, uvs);
                newMesh.SetUVs(i, uvs);
            }
            
            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                var triangles = mesh.GetTriangles(i);
                newMesh.SetTriangles(triangles, i);
            }

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
        
        public static Mesh ScaleFromCenter(this Mesh mesh, Vector3 scale)
        {
            var vertices = mesh.vertices;
            var originalBounds = mesh.bounds;

            for (var i = 0; i < vertices.Length; i++)
            {
                var relativeToCenter =  vertices[i] - originalBounds.center;

                var stretched = new Vector3(
                    relativeToCenter.x * scale.x,
                    relativeToCenter.y * scale.y,
                    relativeToCenter.z * scale.z
                );

                vertices[i] = originalBounds.center + stretched;
            }
            mesh.vertices = vertices;
            mesh.bounds = new Bounds(originalBounds.center, Vector3.Scale(originalBounds.size, scale));
            return mesh;
        }

        public static Mesh UpdateUvs(this Mesh mesh, Vector2 offset, Vector2 scale)
        {
            var uvs = mesh.uv;
            for (var i = 0; i < uvs.Length; i++)
            {
                uvs[i] = uvs[i] * scale + offset;
            }
            mesh.uv = uvs;
            
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
            m.vertices = ClampMeshToBounds(m.vertices, bounds);
            m.RecalculateBounds();
            return m;
        }
        
        public static Vector3[] ClampMeshToBounds(this Vector3[] vertices, Bounds bounds)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = vertices[i].Clamp(bounds.min, bounds.max);
            }
            return vertices;
        }
        
        public static Vector3[] ClampMeshToOtherLocalBounds(this Vector3[] vertices, Transform transform, Transform otherTransform, Bounds otherLocalBounds)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                var thisLocalVertex = vertices[i];
                var otherLocalVertex = transform.TransformPoint(thisLocalVertex, otherTransform);
                var clampedOtherLocalVertex = otherLocalVertex.Clamp(otherLocalBounds.min, otherLocalBounds.max);
                var clampedThisLocalVertex = otherTransform.TransformPoint(clampedOtherLocalVertex, transform);
                vertices[i] = clampedThisLocalVertex;
            }
            return vertices;
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

        public static void RemoveVertices(this Mesh mesh, Predicate<(Vector3 vextex, int index)> predicate)
        {
            var verticesIndexesToDelete = mesh.vertices
                .Select((vertex, index) => (vertex, index))
                .Where(p => predicate((p.vertex, p.index)))
                .Select(p => p.index)
                .ToSet();
            
            mesh.RemoveVertices(verticesIndexesToDelete);
        }
        
        public static void RemoveVertices(this Mesh mesh, ISet<int> vertexIndexes)
        {
            if (mesh.subMeshCount > 1)
            {
                throw new Exception("Only meshes with one submesh are supported.");
            }

            var orderedVertexIndexes = vertexIndexes.OrderBy(i => i).ToArray();
            var correctionOffsets = new List<((int from, int to) range, int offset)>();
            var o = 0;
            for (var i = 0; i < orderedVertexIndexes.Length; i++)
            {
                var vertexIndex = orderedVertexIndexes[i];
                
                if (i > 0)
                {
                    var lastRange = correctionOffsets[i - 1];
                    lastRange.range.to = vertexIndex;
                    correctionOffsets[i - 1] = lastRange;
                }

                var range = (i: vertexIndex, mesh.vertices.Length);
                correctionOffsets.Add((range, --o));
            }

            T[] stripByIndex<T>(T[] array, ISet<int> skip)
            {
                if (array == null)
                    return null;

                var newArray = new List<T>();
                var length = array.Length;
                for (var i = 0; i < length; i++)
                {
                    if (skip.Contains(i) == false)
                    {
                        newArray.Add(array[i]);
                    }
                }

                return newArray.ToArray();
            }

            var newTrianges = new List<int>();
            var oldTriangels = mesh.triangles;
            for (var i = 0; i < oldTriangels.Length; i += 3)
            {
                var t0 = oldTriangels[i + 0];
                var t1 = oldTriangels[i + 1];
                var t2 = oldTriangels[i + 2];
                if (vertexIndexes.Contains(t0) == false &&
                    vertexIndexes.Contains(t1) == false &&
                    vertexIndexes.Contains(t2) == false)
                {
                    newTrianges.Add(t0);
                    newTrianges.Add(t1);
                    newTrianges.Add(t2);
                }
            }
            
            for (var i = 0; i < newTrianges.Count; i++)
            {
                var t = newTrianges[i];
                foreach (var (range, offset) in correctionOffsets)
                {
                    if (t >= range.from && t < range.to)
                    {
                        newTrianges[i] = t + offset;
                    }
                }
            }

            var stripedVertices = stripByIndex(mesh.vertices, vertexIndexes);
            var stripedNormals = stripByIndex(mesh.normals, vertexIndexes);
            
            mesh.triangles = newTrianges.ToArray();
            mesh.vertices = stripedVertices;
            for (var i = 0; i < 8; i++)
            {
                var list = new List<Vector4>();
                mesh.GetUVs(i, list);
                var strippedUvsVector4 = stripByIndex(list.ToArray(), vertexIndexes);
                if (strippedUvsVector4.Length == stripedVertices.Length)
                {
                    mesh.SetUVs(i, strippedUvsVector4);
                }
            }
            mesh.normals = stripedNormals;
        }
        
        public static void RemoveTriangles(this Mesh mesh, ISet<int> triangleIndexes)
        {
            // if (mesh.subMeshCount > 1)
            // {
            //     throw new Exception("Only meshes with one submesh are supported.");
            // }

            var triangles = mesh.triangles;
            var newTriangles = new int[triangles.Length - triangleIndexes.Count*3];
            int addedTriangles = 0;
            for (int i = 0; i < triangles.Length; i+=3)
            {
                var triangleIndex = i / 3;
                if (triangleIndexes.Contains(triangleIndex) == false)
                {
                    newTriangles[addedTriangles + 0] = triangles[i + 0];
                    newTriangles[addedTriangles + 1] = triangles[i + 1];
                    newTriangles[addedTriangles + 2] = triangles[i + 2];
                    addedTriangles += 3;
                }
            }

            mesh.triangles = newTriangles;
        }
        
        public static void TransformVertices(this Mesh mesh, Func<(Vector3 vextex, int index), Vector3> transform)
        {
            mesh.vertices = mesh.vertices.Select((v, i) => transform((v, i))).ToArray();
        }
        
        public static void TransformNormals(this Mesh mesh, Func<(Vector3 normal, int index), Vector3> transform)
        {
            mesh.normals = mesh.normals.Select((v, i) => transform((v, i))).ToArray();
        }
        
        public static void TransformNormals2(this Mesh mesh, Func<(Vector3 normal, Vector3 vertex, int index), Vector3> transform)
        {
            var vertices = mesh.vertices;
            mesh.normals = mesh.normals.Select((v, i) => transform((v, vertices[i], i))).ToArray();
        }

        public static Vector3 Centroid(this List<Mesh> meshes, out float volume)
        {
            Vector3 centroid = new Vector3();
            volume = 0;

            foreach (var mesh in meshes)
            {
                var (c, v) = mesh.Centroid();
                volume += v;
                centroid += v * c;
            }

            return centroid / volume;
        }

        public static (Vector3 centroid, float volume) Centroid(this Mesh mesh)
        {
            var centroid = new Vector3();
            float totalArea = 0;
            var volume = 0f;

            var tris = mesh.triangles;
            var verts = mesh.vertices;
            
            for (int i = 0; i < tris.Length; i += 3)
            {
                var a = verts[tris[i + 0]];
                var b = verts[tris[i + 1]];
                var c = verts[tris[i + 2]];
                var triangleArea = AreaOfTriangle(a, b, c);
                totalArea += triangleArea;
                centroid += triangleArea * (a + b + c) / 3;

                volume += SignedVolumeOfTetrahedron(a, b, c);
            }

            return (centroid / totalArea, volume);
        }

        private static float SignedVolumeOfTetrahedron(Vector3 a, Vector3 b, Vector3 c)
        {
            return (float) (Vector3.Dot(a, Vector3.Cross(b, c)) / 6.0d);
        }

        private static float AreaOfTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            return (float) (0.5d * Vector3.Cross(b - a, c - a).magnitude);
        }

        public static Mesh OffsetSimpleMesh(this Mesh mesh, Vector3 size, float center = 0.5f)
        {
            return mesh.OffsetSimpleMesh(size, Vector3.one * center);
        }

        public static Mesh OffsetSimpleMesh(this Mesh mesh, Vector3 size, Vector3 center)
        {
            var newMesh = mesh.Clone();
            var bounds = newMesh.bounds;
            var vertices = newMesh.vertices;
            var threshold = bounds.min.Lerp( bounds.max, center);
            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = vertices[i];
                var x = vert.x > threshold.x ? vert.x + size.x : vert.x;
                var y = vert.y > threshold.y ? vert.y + size.y : vert.y;
                var z = vert.z > threshold.z ? vert.z + size.z : vert.z;
                vertices[i] = new Vector3(x, y, z);
            }

            newMesh.vertices = vertices;
            newMesh.RecalculateBounds();
            return newMesh;
        }

        public static MeshTransform Transform(this Mesh mesh)
        {
            return new MeshTransform(mesh);
        }

        public static Mesh FlipTriangles(this Mesh mesh)
        {
            var t = mesh.triangles;
            for (int i = 0; i < t.Length; i += 3)
            {
                (t[i + 2], t[i + 1]) = (t[i + 1], t[i + 2]);
            }
            mesh.triangles = t;

            return mesh;
        }

        public static Vector2[] VerticesXZ(this Mesh mesh)
        {
            var vertices = mesh.vertices;
            var result = new Vector2[vertices.Length];
            for (var i = 0; i < vertices.Length; i++)
            {
                var vertex = vertices[i];
                result[i] = new Vector2(vertex.x, vertex.z);
            }
            return result;
        }
    }
}