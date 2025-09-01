using UnityEngine;

namespace ElasticSea.Framework.Util
{
    public static class IntermediateMeshExtensions
    {
        public static IntermediateMesh Create(this Mesh mesh)
        {
            var intermediateMesh = new IntermediateMesh(mesh);
            return intermediateMesh;
        }
    }
}