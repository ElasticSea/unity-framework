using System;
using System.Collections.Generic;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Ui.Layout.Spatial;
using ElasticSea.Framework.Util;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;
using UnityEngine.Serialization;
using static ElasticSea.Framework.Util.Utils;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIconFactory : MonoBehaviour, ILayoutComponent
    {
        [SerializeField, CustomObjectPicker(typeof(ISpatialLayout)), FormerlySerializedAs("_placementGrid")] private Component _spatialLayout;
        public IUniformSpatialLayout SpatialLayout => _spatialLayout as IUniformSpatialLayout;

        [SerializeField] private Mesh circleMesh;
        [SerializeField] private Material circleMaterial;
        [SerializeField] private Transform container;

        private Mesh circleMeshInstance;
        private float circleMeshRadius = -1;

        public FlatMeshIcon[] Build(FlatMeshIconData[] icons, Dictionary<string, FlatMeshIconMeshData> meshIconCache)
        {
            Clear();

            if (icons.Length != SpatialLayout.Count)
            {
                if (SpatialLayout is IUniformGrowSpatialLayout uniformGrowSpatialLayout)
                {
                    uniformGrowSpatialLayout.SetCount(icons.Length);
                }
            }

            var radius = SpatialLayout.Size.FromXY().Min() / 2;

            if (circleMeshRadius != radius)
            {
                circleMeshInstance = circleMesh.Clone();
                GenerateBackplateMesh(circleMeshInstance, SpatialLayout.Size);
                circleMeshRadius = radius;
            }
            
            var translucentIcons = new FlatMeshIcon[icons.Length];
            for (var i = 0; i < icons.Length; i++)
            {
                var icon = icons[i];
                var cell= SpatialLayout.GetCell(i);

                var anchor = new GameObject(icon.Name);
                anchor.transform.SetParent(container, false);
                anchor.transform.localPosition = cell.cellToLocal.GetPosition() + cell.localBounds.center;
                anchor.transform.localRotation = cell.cellToLocal.rotation;

                var dat = GenerateMeshes(icon, meshIconCache);
                
                var translucentIcon = anchor.AddComponent<FlatMeshIcon>();
                translucentIcon.Index = i;
                translucentIcon.Name = icon.Name;
                translucentIcon.BackplateRect = CenterRect(Vector2.zero, SpatialLayout.Size); 
                translucentIcon.Backplate = GenerateBackplate(translucentIcon, circleMeshInstance, circleMaterial, icon.Locked, dat.AccentColor);
                translucentIcon.Collider = translucentIcon.GetComponent<Collider>();
                translucentIcon.FrontplateCircle = (Vector2.zero, radius - icon.Padding);
                var frontplate = GenerateFrontplate(translucentIcon.Backplate, radius - icon.Padding, icon.Locked, circleMeshInstance, dat);
                translucentIcon.Frontplate = frontplate.frontplate;
                translucentIcon.FrontplateRect = frontplate.rect;
                
                translucentIcons[i] = translucentIcon;  
            }

            OnRectChanged?.Invoke();
            
            return translucentIcons;
        }

        private GameObject GenerateBackplate(FlatMeshIcon icon, Mesh circleMesh, Material circleMaterial, bool locked, Color accentColor)
        {
            var backplate = new GameObject("Backplate");

            icon.gameObject.AddComponent<MeshCollider>().sharedMesh = circleMesh;
            backplate.AddComponent<MeshFilter>().sharedMesh = circleMesh;
            var mat = Instantiate(circleMaterial);
            mat.SetColor("_Accent", accentColor);
            backplate.AddComponent<MeshRenderer>().sharedMaterial = mat;
            
            backplate.transform.SetParent(icon.transform, false);
            
            icon.Material = mat;
            
            if (locked)
            {
                mat.SetBool("_Disabled", true);
            }

            return backplate;
        }

        private void GenerateBackplateMesh(Mesh mesh, Vector2 size)
        {
            var vertices = mesh.vertices;
            var verticesLength = vertices.Length;
            var center2d = mesh.bounds.center.FromXY();
            var smallerSide = size.Min();
            var biggerSide = size.Max();
            var offset = (smallerSide - Mathf.Max(mesh.bounds.size.x, mesh.bounds.size.y)) / 2;
            for (var i = 0; i < verticesLength; i++)
            {
                var vertex = vertices[i];
                var vertex2d = new Vector2(vertex.x, vertex.y);
                var extent = (vertex2d - center2d).normalized * offset;
                float horizontalOffset = (biggerSide - mesh.bounds.size.x) * 0.5f - offset;
                var sign = vertex2d.x < center2d.x ? -1 : 1;
                horizontalOffset *= sign;
                vertices[i] = vertex + new Vector3(extent.x + horizontalOffset, extent.y);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }

        private (GameObject frontplate, Rect rect) GenerateFrontplate(GameObject backplate, float radius, bool locked, Mesh circleMesh, FlatMeshIconMeshData dat)
        {
            var bounds = dat.SphereBounds;

            var frontPlate = new GameObject("Frontplate");
            frontPlate.transform.SetParent(backplate.transform, false);
            
            var scaleAnchor = new GameObject("Scaled");
            scaleAnchor.transform.SetParent(frontPlate.transform, false);
            scaleAnchor.transform.localPosition = dat.Offset.ToXy().SetZ( -circleMesh.bounds.max.z);

            var setGo = new GameObject("Mesh");
            setGo.transform.SetParent(scaleAnchor.transform, false);
            setGo.AddComponent<MeshFilter>().sharedMesh = dat.Mesh;

            var addComponent = setGo.AddComponent<MeshRenderer>();
            addComponent.sharedMaterials = locked ? dat.LockedMaterials : dat.Materials;

            setGo.transform.localPosition = -bounds.center.SetZ(0);

            var scale = radius / bounds.radius;
            scaleAnchor.transform.localScale = Vector3.one * scale;
            
            var c = (bounds.center + setGo.transform.localPosition) * scale;
            var r = bounds.radius * Mathf.Abs(scale);
            var rect = new Rect(c.x - r, c.y - r, r * 2f, r * 2f);
            
            return (frontPlate, rect);
        }

        private FlatMeshIconMeshData GenerateMeshes(FlatMeshIconData meshData, Dictionary<string, FlatMeshIconMeshData> meshIconCache)
        {
            if (meshData.MeshDataUniqueId != null && meshIconCache != null)
            {
                if (meshIconCache.TryGetValue(meshData.MeshDataUniqueId, out var meshDatav))
                {
                    return meshDatav;
                }
            }

            var dataa = meshData.MeshDataFactory();

            SquashMesh(dataa.Mesh, dataa.Rotation, dataa.Thickness);
            if (dataa.Mesh != dataa.LowPoly)
            {
                SquashMesh(dataa.LowPoly, dataa.Rotation, dataa.Thickness);
            }
            
            dataa.SphereBounds = dataa.LowPoly.vertices.ToSphereBounds();

            if (meshData.MeshDataUniqueId != null && meshIconCache != null)
            {
                meshIconCache[meshData.MeshDataUniqueId] = dataa;
            }

            return dataa;
        }

        private void SquashMesh(Mesh mesh, Quaternion rotate, float thickness)
        {
            // plane: z = 0 with normal (0,0,-1)
            var vertices = mesh.vertices;
            var normals  = mesh.normals;
            int count    = vertices.Length;

            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            // 1) Rotate verts & normals, find min/max distance in one pass.
            for (int i = 0; i < count; i++)
            {
                var v = rotate * vertices[i];
                vertices[i] = v;
                normals[i]  = rotate * normals[i];

                // distance to plane with normal (0,0,-1) is just -z
                float d = -v.z;
                if (d < min) min = d;
                if (d > max) max = d;
            }

            float range    = max - min;
            float invRange = range > 1e-8f ? 1f / range : 0f; // avoid div by zero

            // 2) Project + thicken in second pass (no Plane calls).
            for (int i = 0; i < count; i++)
            {
                var v = vertices[i];

                // inverse lerp of distance
                float d = -v.z;
                float t = (d - min) * invRange; // Mathf.InverseLerp(min, max, d)

                // project onto plane z=0, then push along -Z by thickness * t
                v.z = -thickness * t;
                vertices[i] = v;
            }

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.RecalculateBounds();
        }

        public void Clear()
        {
            container = container ? container : transform;
            container.DestroyChildren();
        }

        public Rect Rect => SpatialLayout.Bounds.FrontSide();
        public event Action OnRectChanged;
    }
}