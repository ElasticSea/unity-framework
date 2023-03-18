using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class BoxTriangleIntersector
    {
        public static bool IsIntersecting(Bounds box, Vector3 t0, Vector3 t1, Vector3 t2)
        {
            var ibox = new FastBox(box);
            var itriangle = new FastTriangle(t0, t1, t2);

            return IsIntersecting(ibox, itriangle);
        }

        public class FastTriangle
        {
            public readonly Vector3 a;
            public readonly Vector3 b;
            public readonly Vector3 c;
            public readonly Vector3[] vertices;
            public readonly Vector3 normal;

            public FastTriangle(Vector3 t0, Vector3 t1, Vector3 t2)
            {
                this.a = t0;
                this.b = t1;
                this.c = t2;
                this.vertices = new[] { t0, t1, t2 };
                this.normal = new Plane(t0, t1, t2).normal;
            }
        }

        public class FastBox
        {
            public readonly Vector3 min;
            public readonly Vector3 max;
            public readonly Vector3[] vertices;
            public readonly float[] minCoords;
            public readonly float[] maxCoords;

            public FastBox(Bounds bounds)
            {
                min = bounds.min;
                max = bounds.max;
                vertices = bounds.GetVertices();
                minCoords = new[] { min.x, min.y, min.z };
                maxCoords = new[] { max.x, max.y, max.z };
            }
        }

        private static readonly Vector3[] boxNormals =
        {
            new(1, 0, 0),
            new(0, 1, 0),
            new(0, 0, 1)
        };

        public static bool IsIntersecting(FastBox box, FastTriangle fastTriangle)
        {
            float triangleMin, triangleMax;
            float boxMin, boxMax;

            // Test the box normals (x-, y- and z-axes)
            for (int i = 0; i < 3; i++)
            {
                Project(fastTriangle.vertices, boxNormals[i], out triangleMin, out triangleMax);
                if (triangleMax < box.minCoords[i] || triangleMin > box.maxCoords[i])
                    return false; // No intersection possible.
            }

            // Test the triangle normal
            var triangleOffset = Vector3.Dot(fastTriangle.normal, fastTriangle.a);
            Project(box.vertices, fastTriangle.normal, out boxMin, out boxMax);
            if (boxMax < triangleOffset || boxMin > triangleOffset)
                return false; // No intersection possible.

            // Test the nine edge cross-products
            Vector3[] triangleEdges = {
                fastTriangle.a - fastTriangle.b,
                fastTriangle.b - fastTriangle.c,
                fastTriangle.c - fastTriangle.a
            };
            for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                // The box normals are the same as it's edge tangents
                var axis = Vector3.Cross(triangleEdges[i], boxNormals[j]);
                Project(box.vertices, axis, out boxMin, out boxMax);
                Project(fastTriangle.vertices, axis, out triangleMin, out triangleMax);
                if (boxMax <= triangleMin || boxMin >= triangleMax)
                    return false; // No intersection possible
            }

            // No separating axis found.
            return true;
        }

        static void Project(Vector3[] points, Vector3 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;
            var length = points.Length;
            for (var i = 0; i < length; i++)
            {
                var point = points[i];
                var val = Vector3.Dot(axis, point);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }
    }
}