using System;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIconData
    {
        public string Name;
        public bool Locked;
        public float Padding;
        public string MeshDataUniqueId;
        public Func<FlatMeshIconMeshData> MeshDataFactory;
    }

    public class FlatMeshIconMeshData
    {
        public Color AccentColor;
        public Mesh Mesh; // will be modified
        public Mesh LowPoly; // will be modified
        public (Vector3 center, float radius) SphereBounds;
        public Material[] Materials;
        public Material[] LockedMaterials;
        public Quaternion Rotation;
        public Vector2 Offset;
        public float Thickness = 0.0025f;
    }
}