using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Placement
{
    public class ArchedPlacementGrid : MonoBehaviour, IPlacementGrid
    {
        [SerializeField] private float radius;
        [SerializeField] private float width;
        [SerializeField] private float height;
        [SerializeField] private float depth;
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        [SerializeField] private Align horizontalAlign = Align.Center;
        [SerializeField] private Align verticalAlign = Align.Center;

        public int Rows
        {
            get => rows;
            set => rows = value;
        }

        public int Columns
        {
            get => columns;
            set => columns = value;
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < Count; i++)
            {
                DrawArc(i);
            }
            
            PlacementGridUtils.DrawCells(this, Count, transform);
        }

        Vector3 GetPosition(float angle, int row)
        {
            var posy = 0f;
            switch (verticalAlign)
            {
                case Align.Start:
                    posy = (row) * height;
                    break;
                case Align.Center:
                    posy = (row - rows / 2f) * height;
                    break;
                case Align.End:
                    posy = (row - rows) * height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var posx = Mathf.Cos(angle) * radius;
            var posz = Mathf.Sin(angle) * radius;
            return new Vector3(posx, posy, posz);
        }

        private (float start, float end) GetStartEndAngles(int index)
        {
            var x = index % columns;
            var startAngle = 90 * Mathf.Deg2Rad;
            
            var sizeAngle = 2 * Mathf.Asin(width / (2 * radius));

            switch (horizontalAlign)
            {
                case Align.Start:
                    break;
                case Align.Center:
                    startAngle += columns * sizeAngle * 0.5f;
                    break;
                case Align.End:
                    startAngle += columns * sizeAngle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var start = startAngle - (x + 0) * sizeAngle;
            var end = startAngle - (x + 1) * sizeAngle;
            return (start, end);
        }

        private void DrawArc(int index)
        {
            var y = index / columns;
            
            var (a0, a1) = GetStartEndAngles(index);

            var angleDiff = a1 - a0;

            UnityEngine.Gizmos.matrix = transform.localToWorldMatrix;
            UnityEngine.Gizmos.color = Color.yellow;
            
            var segments = 32;
            for (int i = 0; i < segments; i++)
            {
                var ang0 = (i + 0f) / (segments) * angleDiff;
                var ang1 = (i + 1f) / (segments) * angleDiff;
                var posA = GetPosition(a0 + ang0, y);
                var posB = GetPosition(a0 + ang1, y);
                UnityEngine.Gizmos.DrawLine(posA, posB);
            }
        }

        public int Count
        {
            get => columns * rows;
            set => throw new NotImplementedException();
        }

        public Vector3 Size
        {
            get => new(width, height, depth);
            set
            {
                width = value.x;
                height = value.y;
                depth = value.z;
            }
        }

        public Bounds Bounds => default;

        public (Matrix4x4 cellToLocal, Bounds bounds) GetCell(int index)
        {
            var y = index / columns;
            
            var (a0, a1) = GetStartEndAngles(index);

            var posA = GetPosition(a0, y);
            var posB = GetPosition(a1, y);

            var rotation = Quaternion.Euler(0, -Mathf.Lerp(a0, a1, 0.5f) * Mathf.Rad2Deg + 90, 0);
            var boxSize = new Vector3(width, height, depth);
            var centerLine = posA.Lerp(posB, 0.5f);
            var center = centerLine + Vector3.up * height / 2 + centerLine.SetY(0).normalized * depth/2;
            
            var trs = Matrix4x4.TRS(center, rotation, Vector3.one);
            var bounds = new Bounds(Vector3.zero, boxSize);

            return (trs, bounds);
        }
    }
}