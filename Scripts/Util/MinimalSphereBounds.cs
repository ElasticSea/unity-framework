using System.Collections.Generic;
using ElasticSea.Framework.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ElasticSea.Framework.Scripts.Util
{
    public class MinimalSphereBounds
    {
        private static List<Vector3> _boundaryPoints;
        
        public static SphereBounds GetSphereBounds(List<Vector3> points)
        {
            if (_boundaryPoints == null)
                _boundaryPoints = new List<Vector3>(4);
            else
                _boundaryPoints.Clear();

            return BallWithBounds(points);
        }

        static SphereBounds BallWithBounds(List<Vector3> contained)
        {
            if (contained.Count == 0 || _boundaryPoints.Count == 4)
            {
                switch (_boundaryPoints.Count)
                {
                    case 0:
                        return new SphereBounds(Vector3.zero, 0);
                    case 1:
                        return new SphereBounds(_boundaryPoints[0], 0);
                    case 2:
                        var halfSpan = 0.5f * (_boundaryPoints[1] - _boundaryPoints[0]);
                        return new SphereBounds(
                            _boundaryPoints[0] + halfSpan,
                            halfSpan.magnitude
                        );
                    case 3:
                        return TriangleCircumSphere(
                            _boundaryPoints[0],
                            _boundaryPoints[1],
                            _boundaryPoints[2]
                        );
                    case 4:
                        return TetrahedronCircumSphere(
                            _boundaryPoints[0],
                            _boundaryPoints[1],
                            _boundaryPoints[2],
                            _boundaryPoints[3]
                        );
                }
            }

            int last = contained.Count - 1;
            int removeAt = Random.Range(0, contained.Count);

            Vector3 removed = contained[removeAt];
            contained[removeAt] = contained[last];
            contained.RemoveAt(last);

            var ball = BallWithBounds(contained);

            if (!ball.Contains(removed))
            {
                _boundaryPoints.Add(removed);
                ball = BallWithBounds(contained);
                _boundaryPoints.RemoveAt(_boundaryPoints.Count - 1);
            }

            contained.Add(removed);
            return ball;
        }

        static SphereBounds TriangleCircumSphere(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 A = a - c;
            Vector3 B = b - c;

            Vector3 cross = Vector3.Cross(A, B);

            Vector3 center = c + Vector3.Cross(A.sqrMagnitude * B - B.sqrMagnitude * A, cross) / (2f * cross.sqrMagnitude);

            float radius = Vector3.Distance(a, center);

            return new SphereBounds(center, radius);
        }

        static SphereBounds TetrahedronCircumSphere(
            Vector3 p1,
            Vector3 p2,
            Vector3 p3,
            Vector3 p4
        )
        {
            // Construct a matrix with the vectors as columns 
            // (Xs on one row, Ys on the next... and the last row all 1s)
            Matrix4x4 matrix = new Matrix4x4(p1, p2, p3, p4);
            matrix.SetRow(3, Vector4.one);

            float a = matrix.determinant;

            // Copy the matrix so we can modify it 
            // and still read rows from the original.
            var D = matrix;
            Vector3 center;

            Vector4 squares = new Vector4(
                p1.sqrMagnitude,
                p2.sqrMagnitude,
                p3.sqrMagnitude,
                p4.sqrMagnitude
            );

            D.SetRow(0, squares);
            center.x = D.determinant;

            D.SetRow(1, matrix.GetRow(0));
            center.y = -D.determinant;

            D.SetRow(2, matrix.GetRow(1));
            center.z = D.determinant;

            center /= 2f * a;
            return new SphereBounds(center, Vector3.Distance(p1, center));
        }
    }
}