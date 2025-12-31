using System;
using System.Diagnostics;
using System.Linq;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util;
using UnityEngine;
using Random = System.Random;

namespace ElasticSea.Framework.Scripts.Misc
{
    public class BoundsTest : MonoBehaviour
    {
        public enum TestType
        {
            FastCircle, Circle
        }

        [SerializeField] private int pointsCount;
        [SerializeField] private TestType Type;
        public TimeSpan Elapsed { get; private set; }

        private void OnDrawGizmos()
        {
            var transforms = transform.Children();
            if (transforms.Any())
            {
                foreach (var t in transforms)
                {
                    Gizmos.DrawWireSphere(t.position, 0.01f);
                }
                var points = transforms.Select(t => t.position).ToArray();

                Gizmos.color = Color.yellow;
            
                switch (Type)
                {
                    case TestType.FastCircle:
                        DrawCircle(points, BoundsExtensions.ToFastCircleBounds);
                        break;
                    case TestType.Circle:
                        DrawCircle(points, BoundsExtensions.ToCircleBounds);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void DrawCircle(Vector3[] points,  Func<Vector2[], CircleBounds> factory)
        {
            var points2d2 = points.Select(p => new Vector2(p.x, p.z)).ToArray();

            var sw = Stopwatch.StartNew();
            var circle2 = factory(points2d2);
            Elapsed = sw.Elapsed;
            Gizmos.DrawWireSphere(circle2.Center.ToXZ(), 0.01f);

            GizmoUtils.DrawCircle(circle2.Center.ToXZ(), Vector3.up, circle2.Radius);
        }

        public void SpawnPoints()
        {
            transform.DestroyChildren(true);

            var rng = new Random();
            for (int i = 0; i < pointsCount; i++)
            {
                var point = new GameObject("Point " + (i + 1));
                point.transform.SetParent(transform, false);

                var angle = (float)(rng.NextDouble() * Mathf.PI * 2);
                var radius = (float)rng.NextDouble();
                var x = Mathf.Cos(angle) * radius;
                var y = Mathf.Sin(angle) * radius;
                point.transform.position = new Vector3(x, 0, y);
            }
        }
    }
}
