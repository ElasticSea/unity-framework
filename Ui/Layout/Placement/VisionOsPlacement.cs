using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Ui.Layout.Alignment;
using ElasticSea.Framework.Util;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Placement
{
    public class VisionOsPlacement : MonoBehaviour, IPlacement
    {
        [field: SerializeField] public int Count { get; set; }

        public Vector3 Size
        {
            get => new(Radius * 2, Radius * 2, 0);
        }

        public float Radius;
        public float Offset;
        public Align HorizontalAlign = Align.Center;
        public Align VerticalAlign = Align.Center;
        public bool padRight = false;

        private const float CircleVerticalFactor = 0.8660254f; // Mathf.Sqrt(0.75f);
        
        private Rows CalculateRows(int count)
        {
            switch (count)
            {
                case 0:
                    return new Rows();
                case 1:
                    return new Rows { Middle = 1 };
                case 2:
                    return new Rows { Middle = 2 };
                case 3:
                    return new Rows { Middle = 2, Bottom = 1 };
                case 4:
                    return new Rows { Top = 2, Middle = 2 };
                case 5:
                    return new Rows { Top = 2, Middle = 3 };
                case 6:
                    return new Rows { Top = 2, Middle = 3, Bottom = 1 };
                case 7:
                    return new Rows { Top = 2, Middle = 3, Bottom = 2 };
            }

            // For Count > 7
            int extra = count - 7;
            int r0 = extra / 3;
            int r1 = extra / 3;
            int r2 = extra / 3;

            if (extra % 3 == 1)
            {
                r0++;
            }
            else if (extra % 3 == 2)
            {
                r0++;
                r1++;
            }

            return new Rows
            {
                Top = 2 + r0,
                Middle = 3 + r1,
                Bottom = 2 + r2
            };
        }
        
        private class Rows
        {
            public int Top;
            public int Middle;
            public int Bottom;

            public enum RowType
            {
                Top,
                Middle,
                Bottom
            }

            public (RowType type, int index) GetIndex(int index)
            {
                var type = RowType.Top;
                if (Top <= index)
                {
                    index -= Top;
                    type = RowType.Middle;

                    if (Middle <= index)
                    {
                        index -= Middle;
                        type = RowType.Bottom;
                        
                        if (Bottom <= index)
                        {
                            throw new Exception($"Index {index} out of range");
                        }
                    }
                }

                return (type, index);
            }

            public int Count => Top + Middle + Bottom;
        }

        public Bounds Bounds
        {
            get
            {
                var count = Count;
                
                if (padRight && count > 3)
                {
                    if (count > 5)
                    {
                        var remained = count - 7;
                        count = Mathf.Max(7, Mathf.CeilToInt(remained / 3f) * 3 + 7);
                    }
                    else
                    {
                        count = 5;
                    }
                }
                
                var rows = CalculateRows(count);

                if (rows.Count <= 0)
                {
                    return default;
                }

                var min = GetBoundsMin(rows);
                var max = GetBoundsMax(rows);
                var size = max - min;

                var offsetx = (HorizontalAlign.GetAlignDelta() - 1f) * size.x;
                var offsety = (VerticalAlign.GetAlignDelta() - 1f) * size.y;
                var offset = new Vector2(offsetx, offsety);

                var bounds = new Bounds(offset + size / 2, size);
                return bounds;
            }
        }

        private Vector2 GetBoundsMin(Rows rows)
        {
            var min = new Vector2();

            if (rows.Middle == 0)
            {
                min.x = 0.5f;
            }
            else
            {
                min.x = 0;
            }

            if (rows.Bottom != 0)
            {
                min.y = 0;
            }
            else if (rows.Middle != 0)
            {
                min.y = 1f * CircleVerticalFactor;
            }
            else
            {
                min.y = 2f * CircleVerticalFactor;
            }

            min *= (Radius * 2 + Offset);
            return min;
        }

        private Vector2 GetBoundsMax(Rows rows)
        {
            var max = new Vector2();

            if (rows.Middle > Mathf.Max(rows.Bottom, rows.Top))
            {
                max.x = rows.Middle ;
            }
            else
            {
                max.x = (Mathf.Max(rows.Bottom, rows.Top) + 0.5f);
            }

            if (rows.Top != 0)
            {
                max.y = 1 + 2f * CircleVerticalFactor;
            }
            else if (rows.Middle != 0)
            {
                max.y = 1 + 1f * CircleVerticalFactor;
            }
            else
            {
                max.y = 0;
            }

            max *= (Radius * 2 + Offset);
            max.x -= Offset;
            max.y -= Offset;
            
            return max;
        }

        public (Matrix4x4 cellToLocal, Bounds bounds) GetCell(int index)
        {
            var rows = CalculateRows(Count);
            var rowIndex = rows.GetIndex(index);

            var bounds = Bounds;

            var offset = new Vector2();
            switch (rowIndex.type)
            {
                case Rows.RowType.Top:
                    offset.x = 0.5f;
                    offset.y = 2 * CircleVerticalFactor;
                    break;
                case Rows.RowType.Middle:
                    offset.y = 1f * CircleVerticalFactor ;
                    break;
                case Rows.RowType.Bottom:
                    offset.x = 0.5f;
                    offset.y = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            offset.x += rowIndex.index;
            

            if (rows.Bottom != 0)
            {
                offset.y -= 0;
            }
            else if (rows.Middle != 0)
            {
                offset.y -= 1f * CircleVerticalFactor;
            }
            else
            {
                offset.y -= 2f * CircleVerticalFactor;
            }

            var scale = Radius * 2 + Offset;
            var xpos = offset.x * scale;
            var ypos = offset.y * scale;


            var cellBounds = new Bounds(new Vector3(Radius, Radius, 0), new Vector3(Radius * 2, Radius * 2, 0));

            var cellMatrix = Matrix4x4.TRS(bounds.min + new Vector3(xpos, ypos, 0), Quaternion.identity, Vector3.one);

            return (cellMatrix, cellBounds);
        }

        [Header("Gizmos")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private bool drawSelected = true;

        private void OnDrawGizmosSelected()
        {
            if (drawGizmos && drawSelected)
            {
                DrawGizmos();
            }
        }

        private void OnDrawGizmos()
        {
            if (drawGizmos && !drawSelected)
            {
                DrawGizmos();
            }
        }

        private void DrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
            
            for (int i = 0; i < Count; i++)
            {
                var (cellToLocal, bounds) = GetCell(i);
                Gizmos.matrix = transform.localToWorldMatrix * cellToLocal;
                Gizmos.color = Color.red;
                var radius = bounds.size.FromXY().Min() / 2;
                GizmoUtils.DrawCircle(bounds.center, Vector3.back, radius);
            }
        }
    }
}