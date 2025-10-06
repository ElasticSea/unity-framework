using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon
{
    public class IconMeshData
    {
        public Mesh Mesh;
        public (Vector3 center, float radius) SphereBounds;
        public Material[] Materials;
    }
}