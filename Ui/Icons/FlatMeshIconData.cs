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
        public Mesh Mesh; // will be modified
        public Mesh LowPoly; // Used for bounds calculation
        public Material[] Materials;
        public Material[] LockedMaterials;
        public Quaternion Rotation;
        public Vector2 Offset;
        public float Thickness = 0.0025f;
    }
}