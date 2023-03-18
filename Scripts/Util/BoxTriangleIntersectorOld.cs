using System.Collections.Generic;
using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Scripts.Util
{
    public class BoxTriangleIntersectorOld
    {
        public static bool IsIntersecting(Bounds box, Vector3 t0, Vector3 t1, Vector3 t2)
        {
            var ibox = new AABoxImpl(box);
            var itriangle = new TriangleImpl(t0, t1, t2);

            return IsIntersecting(ibox, itriangle);
        }

        public class TriangleImpl : ITriangle
        {
            private readonly Vector3 t0;
            private readonly Vector3 t1;
            private readonly Vector3 t2;

            public TriangleImpl(Vector3 t0, Vector3 t1, Vector3 t2)
            {
                this.t0 = t0;
                this.t1 = t1;
                this.t2 = t2;
            }

            public IEnumerable<IVector> Vertices => new[]
            {
                new Vector3Impl(t0),
                new Vector3Impl(t1),
                new Vector3Impl(t2),
            };

            public IVector Normal => new Vector3Impl(new Plane(t0, t1, t2).normal);
            public IVector A => new Vector3Impl(t0);
            public IVector B=> new Vector3Impl(t1);
            public IVector C => new Vector3Impl(t2);
        }

        public class AABoxImpl : IAABox
        {
            private readonly Bounds bounds;

            public AABoxImpl(Bounds bounds)
            {
                this.bounds = bounds;
            }

            public IEnumerable<IVector> Vertices => bounds.GetVertices().Select(v => new Vector3Impl(v));
            public IVector Start => new Vector3Impl(bounds.min);
            public IVector End => new Vector3Impl(bounds.max);
        }

        public static bool IsIntersecting(IAABox box, ITriangle triangle)
        {
            double triangleMin, triangleMax;
            double boxMin, boxMax;

            // Test the box normals (x-, y- and z-axes)
            var boxNormals = new IVector[]
            {
                new Vector3Impl(1, 0, 0),
                new Vector3Impl(0, 1, 0),
                new Vector3Impl(0, 0, 1)
            };
            for (int i = 0; i < 3; i++)
            {
                IVector n = boxNormals[i];
                Project(triangle.Vertices, boxNormals[i], out triangleMin, out triangleMax);
                if (triangleMax < box.Start.Coords[i] || triangleMin > box.End.Coords[i])
                    return false; // No intersection possible.
            }

            // Test the triangle normal
            double triangleOffset = triangle.Normal.Dot(triangle.A);
            Project(box.Vertices, triangle.Normal, out boxMin, out boxMax);
            if (boxMax < triangleOffset || boxMin > triangleOffset)
                return false; // No intersection possible.

            // Test the nine edge cross-products
            IVector[] triangleEdges = new IVector[]
            {
                triangle.A.Minus(triangle.B),
                triangle.B.Minus(triangle.C),
                triangle.C.Minus(triangle.A)
            };
            for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                // The box normals are the same as it's edge tangents
                IVector axis = triangleEdges[i].Cross(boxNormals[j]);
                Project(box.Vertices, axis, out boxMin, out boxMax);
                Project(triangle.Vertices, axis, out triangleMin, out triangleMax);
                if (boxMax <= triangleMin || boxMin >= triangleMax)
                    return false; // No intersection possible
            }

            // No separating axis found.
            return true;
        }

        static void Project(IEnumerable<IVector> points, IVector axis,
            out double min, out double max)
        {
            min = double.PositiveInfinity;
            max = double.NegativeInfinity;
            foreach (var p in points)
            {
                double val = axis.Dot(p);
                if (val < min) min = val;
                if (val > max) max = val;
            }
        }
        
        public class Vector3Impl : IVector
        {
            private readonly Vector3 vec;

            public Vector3Impl(Vector3 vec)
            {
                this.vec = vec;
            }

            public Vector3Impl(int x, int y, int z) : this(new Vector3(x,y,z))
            {
            }


            public float X => vec.x;
            public float Y  => vec.y;
            public float Z => vec.z;
            public float[] Coords => new[] { X, Y, Z };
            public float Dot(IVector other)
            {
                return Vector3.Dot(vec, new Vector3(other.X, other.Y, other.Z));
            }

            public IVector Minus(IVector other)
            {
                return new Vector3Impl(new Vector3(X - other.X, Y - other.Y, Z - other.Z));
            }

            public IVector Cross(IVector other)
            {
                return new Vector3Impl(Vector3.Cross(vec, new Vector3(other.X, other.Y, other.Z)));
            }
        }

        public interface IVector
        {
            float X { get; }
            float Y { get; }
            float Z { get; }
            float[] Coords { get; }
            float Dot(IVector other);
            IVector Minus(IVector other);
            IVector Cross(IVector other);
        }

        public interface IShape
        {
            IEnumerable<IVector> Vertices { get; }
        }

        public interface IAABox : IShape
        {
            IVector Start { get; }
            IVector End { get; }
        }

        public interface ITriangle : IShape
        {
            IVector Normal { get; }
            IVector A { get; }
            IVector B { get; }
            IVector C { get; }
        }
    }
}