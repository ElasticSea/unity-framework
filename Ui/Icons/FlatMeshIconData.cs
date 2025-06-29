using ElasticSea.Framework.Scripts.Util;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIconData
    {
        public string Name;
        public bool Locked;
        public float Padding;
        public Color AccentColor;
        public FlatMeshIconMeshData MeshData;
    }

    public class FlatMeshIconMeshData
    {
        public Mesh Mesh;
        public Material[] Materials;
        public Material[] LockedMaterials;
        public Quaternion MeshRotation;
        public SphereBounds Bounds;
    }
}