using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Ui.Layout;
using ElasticSea.Framework.Ui.Layout.Placement;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icons
{
    public class FlatMeshIconFactory : MonoBehaviour, ILayoutComponent
    {
        [CustomObjectPicker(typeof(IPlacement))]
        [SerializeField] private Component _placementGrid;
        public IPlacement PlacementGrid => _placementGrid as IPlacement;

        private FlatMeshIconData[] icons;

        public FlatMeshIcon[] Build(FlatMeshIconData[] icons, Mesh circleMesh, Material circleMaterial)
        {
            this.icons = icons;
            transform.DestroyChildren();
            
            PlacementGrid.Count = icons.Length;

            var radius = PlacementGrid.Size.FromXY().Min() / 2;
            var circleMeshClone = circleMesh.Clone();
            GenerateBackplateMesh(circleMeshClone, radius);
            
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
                var backplate = GenerateBackplate(translucentIcon, circleMeshClone, circleMaterial, icon.Locked, icon.AccentColor);
                GenerateFrontplate(backplate, radius, icon.Locked, circleMeshClone, icon.MeshData, icon.Padding);
                
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

        private void GenerateBackplateMesh(Mesh mesh, float radius)
        {
            var vertices = mesh.vertices;
            var verticesLength = vertices.Length;
            var center2d = mesh.bounds.center.FromXY();
            var offset = radius - Mathf.Max(mesh.bounds.size.x, mesh.bounds.size.y) / 2;
            for (var i = 0; i < verticesLength; i++)
            {
                var vertex = vertices[i];
                var vertex2d = new Vector2(vertex.x, vertex.y);
                var extent = (vertex2d - center2d).normalized * offset;

                vertices[i] = vertex + new Vector3(extent.x, extent.y);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }

        private GameObject GenerateFrontplate(GameObject backplate, float radius, bool locked, Mesh circleMesh, FlatMeshIconMeshData meshData, float padding)
        {
            var mesh = meshData.Mesh;
            var lowPolymesh = meshData.LowPoly;
            
            var thickness = 0.0025f;
            SquashMesh(mesh, meshData.MeshRotation, thickness);
            SquashMesh(lowPolymesh, meshData.MeshRotation, thickness);
            
            var bounds = lowPolymesh.vertices.ToSphereBounds();
            
            var scaleAnchor = new GameObject("Scaled");
            scaleAnchor.transform.SetParent(backplate.transform, false);
            scaleAnchor.transform.localPosition = new Vector3(0, 0, -circleMesh.bounds.max.z);

            var setGo = new GameObject("Mesh");
            setGo.transform.SetParent(scaleAnchor.transform, false);
            setGo.AddComponent<MeshFilter>().sharedMesh = mesh;

            var addComponent = setGo.AddComponent<MeshRenderer>();
            addComponent.sharedMaterials = locked ? meshData.LockedMaterials : meshData.Materials;

            setGo.transform.localPosition = -bounds.center.SetZ(0);

            var maxCirleRadius = radius - padding;
            scaleAnchor.transform.localScale = Vector3.one * (maxCirleRadius / bounds.radius);
            return scaleAnchor;
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