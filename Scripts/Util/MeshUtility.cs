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
        private readonly Bounds originalBounds;
        private readonly Vector3[] newVertices;
        private readonly Vector2[] newUvs;

        public Mesh Mesh => finalMesh;
        public Mesh Source => source;
        public Bounds Bounds => originalBounds;

        public MeshUtility(Mesh source)
        {
            this.source = source;
            this.originalVertices = source.vertices;
            this.originalBounds = source.bounds;
            this.originalUvs = source.uv;
            this.finalMesh = source.Clone();
            this.newVertices = new Vector3[source.vertices.Length];
            this.newUvs = new Vector2[source.uv.Length];
        }

        public void ExtendMeshToBounds(Bounds boundsToMatch)
        {
            var bounds = originalBounds;
            
            // Center is at 0.5, 0.5, 0.5
            var boundsCenter = bounds.min + bounds.size.Multiply(Vector3.one * 0.5f);

            var minExtends = boundsToMatch.min - bounds.min + boundsCenter - bounds.center;
            var maxExtends = boundsToMatch.max - bounds.max + boundsCenter - bounds.center;

            for (var i = 0; i < originalVertices.Length; i++)
            {
                var vert = originalVertices[i];
                var x = vert.x + (vert.x > boundsCenter.x / 2 ? maxExtends.x : minExtends.x);
                var y = vert.y + (vert.y > boundsCenter.y / 2 ? maxExtends.y : minExtends.y);
                var z = vert.z + (vert.z > boundsCenter.z / 2 ? maxExtends.z : minExtends.z);
                newVertices[i] = new Vector3(x, y, z) ;
            }

            finalMesh.vertices = newVertices;
            finalMesh.bounds = boundsToMatch;
        }

        public void ExtendMeshToBoundsX(float min, float max)
        {
            // Center is at 0.5, 0.5, 0.5
            var boundsCenter = originalBounds.min + originalBounds.size.Multiply(Vector3.one * 0.5f);

            var minExtends = min - originalBounds.min.x + boundsCenter.x - originalBounds.center.x;
            var maxExtends = max - originalBounds.max.x + boundsCenter.x - originalBounds.center.x;

            for (var i = 0; i < originalVertices.Length; i++)
            {
                var vert = originalVertices[i];
                var x = vert.x + (vert.x > boundsCenter.x / 2 ? maxExtends : minExtends);
                var y = vert.y;
                var z = vert.z;
                newVertices[i] = new Vector3(x, y, z);
            }

            finalMesh.vertices = newVertices;

            var newBounds = originalBounds.SetMinMaxX(min, max);
            finalMesh.bounds = newBounds;
        }
    }
}