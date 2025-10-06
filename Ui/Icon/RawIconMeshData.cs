using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon
{
    public class RawIconMeshData
    {
        public Mesh Mesh; // will be modified
        public Mesh LowPoly; // will be modified
        public Material[] Materials;
        public Quaternion Rotation;
        public float Thickness = 0.0025f;
    }
}