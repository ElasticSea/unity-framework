using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class MeshBuilder
    {
        private List<Vector3> vertices = new();
        private List<int> triangles = new();

        public void AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2, bool flip = false)
        {
            var vCount = vertices.Count;
            if (flip)
            {
                (p2, p1) = (p1, p2);
            }

            vertices.Add(p0);
            vertices.Add(p1);
            vertices.Add(p2);
            triangles.Add(vCount + 0);
            triangles.Add(vCount + 1);
            triangles.Add(vCount + 2);
        }

        public void AddQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, bool flip = false)
        {
            AddTriangle(p0, p1, p2, flip);
            AddTriangle(p0, p2, p3, flip);
        }

        public void AddStrip(Vector3[] points0, Vector3[] points1, bool flip = false)
        {
            if (points0.Length != points1.Length)
            {
                throw new Exception("Points needs to be same length");
            }

            var length = points0.Length;
            for (var i = 0; i < length; i++)
            {
                var p00 = points0[i + 0];
                var p01 = points0[(i + 1) % length];
                var p10 = points1[i + 0];
                var p11 = points1[(i + 1) % length];

                AddQuad(p00, p10, p11, p01, flip);
            }
        }

        public void AddCircle(Vector3[] points, Vector3 center, bool flip = false)
        {
            for (var i = 0; i < points.Length; i++)
            {
                var p0 = points[i + 0];
                var p1 = points[(i + 1) % points.Length];
                AddTriangle(p0, center, p1, flip);
            }
        }

        public Mesh GetMesh()
        {
            var mesh = new Mesh()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}