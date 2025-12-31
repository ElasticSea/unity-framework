using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Layout;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class OldFlatSpatialGrid : MonoBehaviour, ISpatialGrid, ILayoutComponent
    {
        public enum MajorAxis { RowMajor, ColumnMajor }
        public enum HorizontalDir { LeftToRight, RightToLeft }
        public enum VerticalDir { BottomToTop, TopToBottom }
        
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        [SerializeField] private Vector3 size;
        [SerializeField] private Vector2 spacing;
        [SerializeField] private Align horizontalAlign = Align.Center;
        [SerializeField] private Align verticalAlign = Align.Center;

        [Header("Cell Order")]
        [SerializeField] private MajorAxis majorAxis = MajorAxis.RowMajor;
        [SerializeField] private HorizontalDir horizontalDir = HorizontalDir.LeftToRight;
        [SerializeField] private VerticalDir verticalDir = VerticalDir.TopToBottom;

        public int Rows { get => rows; set => rows = value; }
        public int Columns { get => columns; set => columns = value; }
        public Vector2 Spacing { get => spacing; set => spacing = value; }
        public Vector3 Size { get => size; set => size = value; }

        public Bounds Bounds
        {
            get
            {
                var xSize = size.x * Columns + spacing.x * (Columns - 1);
                var ySize = size.y * Rows + spacing.y * (rows - 1);
                var x = (0.5f - horizontalAlign.GetAlignDelta()) * xSize;
                var y = (0.5f - verticalAlign.GetAlignDelta()) * ySize;
                var bounds = new Bounds(new Vector3(x, y, size.z / 2), new Vector3(xSize, ySize, size.z));
                return bounds;
            }
        }

        public int Count
        {
            get => this.Count();
        }

        public SpatialCell GetCell(int index)
        {
            var (x, y) = IndexToXY(index);
            
            var xpos = x * (size.x + spacing.x);
            var ypos = y * (size.y + spacing.y);
            
            var cellBounds = new Bounds(size / 2, size);

            var cellMatrix = Matrix4x4.TRS(Bounds.min + new Vector3(xpos, ypos, 0), Quaternion.identity, Vector3.one);

            return new(cellMatrix, cellBounds);
        }
        
        private (int x, int y) IndexToXY(int index)
        {
            int x, y;
            if (majorAxis == MajorAxis.RowMajor)
            {
                x = index % Columns;
                y = index / Columns;
            }
            else
            {
                x = index / Rows;
                y = index % Rows;
            }

            if (horizontalDir == HorizontalDir.RightToLeft)
                x = Columns - 1 - x;

            if (verticalDir == VerticalDir.TopToBottom)
                y = Rows - 1 - y;

            return (x, y);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size * 1.01f);
            
            SpatialLayoutUtils.DrawLayout(this, Count, transform);
        }

        public Rect Rect => Bounds.FrontSide();
        public event Action OnRectChanged;
    }
}