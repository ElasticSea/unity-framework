using System;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class IntermediateMesh
    {
        public readonly Mesh Mesh;
        public readonly Vector3[] OriginalVertices;
        public readonly Bounds OriginalBounds;
        public readonly int Count;
        
        private readonly Vector3[] vertices;
        private Bounds bounds;

        public IntermediateMesh(Mesh originalMesh)
        {
            Mesh = originalMesh.Clone();
            OriginalVertices = Mesh.vertices;
            OriginalBounds = Mesh.bounds;
            Count = OriginalVertices.Length;
            
            vertices = new Vector3[Count];
            Array.Copy(OriginalVertices, vertices, Count);
            bounds = OriginalBounds;
        }

        public void Apply()
        {
            Mesh.vertices = vertices;
            Mesh.bounds = bounds;
        }

        public void Reset()
        {
            Array.Copy(OriginalVertices, vertices, Count);
            bounds = OriginalBounds;
        }

        public void Scale(Vector3 scale)
        {
            for (var i = 0; i < Count; i++)
            {
                var vertex = vertices[i];
                vertex.Scale(scale);
                vertices[i] = vertex;
            }

            bounds = bounds.Scale(scale);
        }

        public void ExtendToBounds(Bounds boundsToMatch)
        {
            var boundsCenter = bounds.center;

            var minExtends = boundsToMatch.min - bounds.min;
            var maxExtends = boundsToMatch.max - bounds.max;

            for (var i = 0; i < vertices.Length; i++)
            {
                var vert = vertices[i];
                var x = vert.x + (vert.x > boundsCenter.x ? maxExtends.x : minExtends.x);
                var y = vert.y + (vert.y > boundsCenter.y ? maxExtends.y : minExtends.y);
                var z = vert.z + (vert.z > boundsCenter.z ? maxExtends.z : minExtends.z);
                vertices[i] = new Vector3(x, y, z) ;
            }

            bounds = boundsToMatch;
        }
    }
}