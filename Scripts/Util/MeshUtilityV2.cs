using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public class MeshUtilityV2
    {
        private readonly Mesh source;
        private readonly Mesh finalMesh;
        private readonly Vector3[] originalVertices;
        private readonly Bounds originalBounds;
        private readonly Vector3[] newVertices;

        public Mesh Mesh => finalMesh;
        public Mesh Source => source;
        public Bounds Bounds => originalBounds;

        public MeshUtilityV2(Mesh source)
        {
            this.source = source;
            this.originalVertices = source.vertices;
            this.originalBounds = source.bounds;
            this.finalMesh = source.Clone();
            this.newVertices = new Vector3[source.vertices.Length];
        }

        public delegate void UpdateVerticesHandler(Vector3[] originalVertices, Vector3[] newVertices);

        public void UpdateVertices(UpdateVerticesHandler factory)
        {
            factory(originalVertices, newVertices);

            finalMesh.vertices = newVertices;
            finalMesh.RecalculateBounds();
        }
    }
}