using System;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class MeshUtility
    {
        private readonly Mesh source;
        private readonly Mesh finalMesh;
        private readonly Vector3[] originalVertices;
        private readonly Vector2[] originalUvs;

        public Mesh Mesh => finalMesh;
        public Mesh Source => source;

        public MeshUtility(Mesh source)
        {
            this.source = source;
            this.originalVertices = source.vertices;
            this.originalUvs = source.uv;
            this.finalMesh = source.Clone();
        }

        public void ExtendVerticesRight(float center, Vector3 extend)
        {
            var length = originalVertices.Length;
            var newVertices = new Vector3[length];
            for (var i = 0; i < length; i++)
            {
                var v = originalVertices[i];
                if (v.x >= center)
                {
                    v += extend;
                }

                newVertices[i] = v;
            }

            finalMesh.vertices = newVertices;
            finalMesh.RecalculateBounds();
        }

        public void ExtendVerticesForward(float center, Vector3 extend)
        {
            var length = originalVertices.Length;
            var newVertices = new Vector3[length];
            for (var i = 0; i < length; i++)
            {
                var v = originalVertices[i];
                if (v.z >= center)
                {
                    v += extend;
                }

                newVertices[i] = v;
            }

            finalMesh.vertices = newVertices;
            finalMesh.RecalculateBounds();
        }

        public void ExtendUvsForward(float center, Vector2 extend)
        {
            var length = originalVertices.Length;
            var newUvs = new Vector2[length];
            for (var i = 0; i < length; i++)
            {
                var v = originalVertices[i];
                var uv = originalUvs[i];
                
                if (v.z >= center)
                {
                    uv += extend;
                }

                newUvs[i] = uv;
            }

            finalMesh.uv = newUvs;
        }
    }
}