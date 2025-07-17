using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Scripts.Extensions;
using ElasticSea.Framework.Ui.Layout;
using ElasticSea.Framework.Ui.Layout.Placement;
using ElasticSea.Framework.Util.PropertyDrawers;
using NanoXLSX;
using UnityEngine;
using static ElasticSea.Framework.Util.Utils;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIconFactory : MonoBehaviour, ILayoutComponent
    {
        [CustomObjectPicker(typeof(IPlacement))]
        [SerializeField] private Component _placementGrid;
        public IPlacement PlacementGrid => _placementGrid as IPlacement;

        [SerializeField] private Mesh circleMesh;
        [SerializeField] private Material circleMaterial;

        public FlatMeshIcon[] Build(FlatMeshIconData[] icons)
        {
            transform.DestroyChildren();

            if (icons.Length != PlacementGrid.Count)
            {
                PlacementGrid.Count = icons.Length;
            }

            var radius = PlacementGrid.Size.FromXY().Min() / 2;
            var circleMeshClone = circleMesh.Clone();
            GenerateBackplateMesh(circleMeshClone, PlacementGrid.Size);
            
            var translucentIcons = new FlatMeshIcon[icons.Length];
            for (var i = 0; i < icons.Length; i++)
            {
                var icon = icons[i];
                var (cellToLocal, cellBounds) = PlacementGrid.GetCell(i);

                var anchor = new GameObject(icon.Name);
                anchor.transform.SetParent(transform, false);
                anchor.transform.localPosition = cellToLocal.GetPosition() + cellBounds.center;
                anchor.transform.localRotation = cellToLocal.rotation;

                var translucentIcon = anchor.AddComponent<FlatMeshIcon>();
                translucentIcon.Index = i;
                translucentIcon.BackplateRect = CenterRect(Vector2.zero, PlacementGrid.Size); 
                translucentIcon.Backplate = GenerateBackplate(translucentIcon, circleMeshClone, circleMaterial, icon.Locked, icon.AccentColor);
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
            var normal = Vector3.back;
            var plane = new Plane(normal, 0);

            var min = float.PositiveInfinity;
            var max = float.NegativeInfinity;

            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var count = vertices.Length;

            for (int i = 0; i < count; i++)
            {
                vertices[i] = rotate * vertices[i];
                normals[i] = rotate * normals[i];
            }

            for (int i = 0; i < count; i++)
            {
                var vertex = vertices[i];
                var distance = plane.GetDistanceToPoint(vertex);
                min = Mathf.Min(min, distance);
                max = Mathf.Max(max, distance);
            }
            
            for (int i = 0; i < count; i++)
            {
                var vertex = vertices[i];
                var distance = plane.GetDistanceToPoint(vertex);

                var distanceDelta = Mathf.InverseLerp(min, max, distance);
                var pointOnPlane = plane.ClosestPointOnPlane(vertex);
                vertices[i] = pointOnPlane + plane.normal * thickness * distanceDelta;
            }
            
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.RecalculateBounds();
        }

        public Rect Rect => PlacementGrid.Bounds.FrontSide();
        public event Action OnRectChanged;
    }
}