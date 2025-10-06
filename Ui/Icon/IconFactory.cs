using System.Collections.Generic;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon
{
    public class IconFactory
    {
        private readonly Mesh backplateMesh;
        private readonly Dictionary<float, Mesh> backplateCache = new();

        public IconFactory(Mesh backplateMesh)
        {
            this.backplateMesh = backplateMesh;
        }

        public Icon BuildIcon(IconData iconData)
        {
            var icon = new GameObject(iconData.Name).AddComponent<Icon>();

            var backplateMesh = GetBackplateMesh(iconData.Radius);
            var frontplateMesh = iconData.ProcessedMesh ?? GenerateMeshes(iconData.RawMesh);
            
            icon.Id = iconData.Name;
            icon.Backplate = GenerateBackplate(icon, backplateMesh);
            icon.Frontplate = GenerateFrontplate(icon, frontplateMesh, iconData.Radius - iconData.Padding);
            icon.Frontplate.transform.SetParent(icon.transform, false);
            icon.Data = frontplateMesh;

            return icon;
        }

        private IconMeshData GenerateMeshes(RawIconMeshData iconDataRawMesh)
        {
            var mesh = iconDataRawMesh.Mesh;
            var lowPoly = iconDataRawMesh.LowPoly;
            var rotation = iconDataRawMesh.Rotation;
            var thickness = iconDataRawMesh.Thickness;

            SquashMesh(mesh, rotation, thickness);
            if (mesh != lowPoly)
            {
                SquashMesh(lowPoly, rotation,thickness);
            }
            
            var sphereBounds = lowPoly.vertices.ToSphereBounds();

            var processedMesh = new IconMeshData
            {
                Mesh = mesh,
                SphereBounds = sphereBounds,
                Materials = iconDataRawMesh.Materials,
            };

            return processedMesh;
        }

        private Mesh GetBackplateMesh(float radius)
        {
            if (!backplateCache.TryGetValue(radius, out var mesh))
            {
                mesh = backplateMesh.Clone();
                GenerateBackplateMesh(mesh, radius);
                backplateCache[radius] = mesh;
            }
            
            return mesh;
        }

        private void GenerateBackplateMesh(Mesh mesh, float radius)
        {
            var vertices = mesh.vertices;
            var vertexCount = vertices.Length;

            var bounds = mesh.bounds;
            var center2d = bounds.center.FromXY();
            var maxExtent = Mathf.Max(bounds.size.x, bounds.size.y);
            var currentRadius = maxExtent * 0.5f;

            // Difference between target radius and current mesh radius
            var offset = radius - currentRadius;

            for (var i = 0; i < vertexCount; i++)
            {
                var vertex = vertices[i];
                var vertex2d = new Vector2(vertex.x, vertex.y);

                // Direction from center to vertex
                var direction = (vertex2d - center2d).normalized;
                if (direction.sqrMagnitude < 1e-6f)
                    continue;

                // Expand or contract outward by offset
                var extent = direction * offset;
                vertices[i] = vertex + new Vector3(extent.x, extent.y, 0f);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }

        private GameObject GenerateBackplate(Icon icon, Mesh circleMesh)
        {
            var backplate = new GameObject("Backplate");
            backplate.transform.SetParent(icon.transform, false);
            backplate.transform.localPosition = new Vector3(0, 0, circleMesh.bounds.max.z);

            var meshCollider = icon.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = circleMesh;
            icon.Collider = meshCollider;
            icon.Backplate = backplate;
            icon.BackplateRenderer = backplate.AddComponent<MeshRenderer>();
            backplate.AddComponent<MeshFilter>().sharedMesh = circleMesh;
            
            return backplate;
        }

        private GameObject GenerateFrontplate(Icon icon, IconMeshData meshIconData, float radius)
        {
            var bounds = meshIconData.SphereBounds;

            var frontPlate = new GameObject("Frontplate");

            var scaleAnchor = new GameObject("Scaled");
            scaleAnchor.transform.SetParent(frontPlate.transform, false);

            var setGo = new GameObject("Mesh");
            setGo.transform.SetParent(scaleAnchor.transform, false);
            setGo.AddComponent<MeshFilter>().sharedMesh = meshIconData.Mesh;

            var frontplateRenderer = setGo.AddComponent<MeshRenderer>();
            frontplateRenderer.sharedMaterials = meshIconData.Materials;
            icon.FrontplateRenderer = frontplateRenderer;

            setGo.transform.localPosition = -bounds.center.SetZ(0);

            var scale = radius / bounds.radius;
            scaleAnchor.transform.localScale = Vector3.one * scale;

            // var c = (bounds.center + setGo.transform.localPosition) * scale;
            // var r = bounds.radius * Mathf.Abs(scale);
            // var rect = new Rect(c.x - r, c.y - r, r * 2f, r * 2f);

            return frontPlate;
        }

        private void SquashMesh(Mesh mesh, Quaternion rotate, float thickness)
        {
            // plane: z = 0 with normal (0,0,-1)
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            int count = vertices.Length;

            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            // 1) Rotate verts & normals, find min/max distance in one pass.
            for (int i = 0; i < count; i++)
            {
                var v = rotate * vertices[i];
                vertices[i] = v;
                normals[i] = rotate * normals[i];

                // distance to plane with normal (0,0,-1) is just -z
                float d = -v.z;
                if (d < min) min = d;
                if (d > max) max = d;
            }

            float range = max - min;
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
    }
}