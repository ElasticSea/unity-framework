using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Ui.Layout.Spatial;
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

        public FlatMeshIcon[] Build(FlatMeshIconData[] icons)
        {
            container = container ? container : transform;
            container.DestroyChildren();

            if (icons.Length != SpatialLayout.Count)
            {
                if (SpatialLayout is IUniformGrowSpatialLayout uniformGrowSpatialLayout)
                {
                    uniformGrowSpatialLayout.SetCount(icons.Length);
                }
            }

            var radius = SpatialLayout.Size.FromXY().Min() / 2;
            var circleMeshClone = circleMesh.Clone();
            GenerateBackplateMesh(circleMeshClone, SpatialLayout.Size);
            
            var translucentIcons = new FlatMeshIcon[icons.Length];
            for (var i = 0; i < icons.Length; i++)
            {
                var icon = icons[i];
                var cell= SpatialLayout.GetCell(i);

                var anchor = new GameObject(icon.Name);
                anchor.transform.SetParent(container, false);
                anchor.transform.localPosition = cell.cellToLocal.GetPosition() + cell.localBounds.center;
                anchor.transform.localRotation = cell.cellToLocal.rotation;

                var translucentIcon = anchor.AddComponent<FlatMeshIcon>();
                translucentIcon.Index = i;
                translucentIcon.Name = icon.Name;
                translucentIcon.BackplateRect = CenterRect(Vector2.zero, SpatialLayout.Size); 
                translucentIcon.Backplate = GenerateBackplate(translucentIcon, circleMeshClone, circleMaterial, icon.Locked, icon.AccentColor);
                translucentIcon.Collider = translucentIcon.GetComponent<Collider>();
                translucentIcon.FrontplateCircle = (Vector2.zero, radius - icon.Padding);
                var frontplate = GenerateFrontplate(translucentIcon.Backplate, radius - icon.Padding, icon.Locked, circleMeshClone, icon.MeshData);
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

        private (GameObject frontplate, Rect rect) GenerateFrontplate(GameObject backplate, float radius, bool locked, Mesh circleMesh, FlatMeshIconMeshData meshData)
        {
            var mesh = meshData.Mesh;
            var lowPolymesh = meshData.LowPoly;
            
            var thickness = 0.0025f;
            SquashMesh(mesh, meshData.Rotation, thickness);
            if (lowPolymesh != mesh)
            {
                SquashMesh(lowPolymesh, meshData.Rotation, thickness);
            }

            var bounds = lowPolymesh.vertices.ToSphereBounds();

            var frontPlate = new GameObject("Frontplate");
            frontPlate.transform.SetParent(backplate.transform, false);
            
            var scaleAnchor = new GameObject("Scaled");
            scaleAnchor.transform.SetParent(frontPlate.transform, false);
            scaleAnchor.transform.localPosition = meshData.Offset.ToXy().SetZ( -circleMesh.bounds.max.z);

            var setGo = new GameObject("Mesh");
            setGo.transform.SetParent(scaleAnchor.transform, false);
            setGo.AddComponent<MeshFilter>().sharedMesh = mesh;

            var addComponent = setGo.AddComponent<MeshRenderer>();
            addComponent.sharedMaterials = locked ? meshData.LockedMaterials : meshData.Materials;

            setGo.transform.localPosition = -bounds.center.SetZ(0);

            var scale = radius / bounds.radius;
            scaleAnchor.transform.localScale = Vector3.one * scale;

            var rect = lowPolymesh.bounds.Move(setGo.transform.localPosition).Scale(scale).FrontSide();
            return (frontPlate, rect);
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

        public Rect Rect => SpatialLayout.Bounds.FrontSide();
        public event Action OnRectChanged;
    }
}