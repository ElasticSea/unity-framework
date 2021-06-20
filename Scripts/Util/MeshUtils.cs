using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public static class MeshUtils
    {
        public static Mesh Quad()
        {
            return new Mesh
            {
                vertices = new[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 1),
                    new Vector3(1, 0, 1),
                    new Vector3(1, 0, 0)
                },
                triangles = new[]
                {
                    0, 1, 2,
                    0, 2, 3
                },
                uv = new[]
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
                normals = new[]
                {
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0)
                }
            };
        }
    }
}