using System;
using System.Linq;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class TightFlatSpatialGrid : ISpatialGrid
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Vector2 Spacing { get; set; }
        public Vector3 Size { get; set; }
        public int Count => this.Count();

        private float[] _columnWidths; // length = Columns
        private float[] _rowHeights;   // length = Rows

        /// <summary>
        /// Apply variable cell sizing from the given Rects:
        /// - Each column width becomes the max Rect.width observed in that column.
        /// - Each row height becomes the max Rect.height observed in that row.
        /// Unspecified cells fall back to the uniform Size.x / Size.y.
        /// Call this whenever rects change. To revert to uniform sizing, call ClearVariableSizing().
        /// </summary>
        public void ApplySizing(Vector2[] sizes)
        {
            if (Columns <= 0 || Rows <= 0)
                throw new InvalidOperationException("Rows and Columns must be > 0 before applying rect sizing.");

            // Initialize with uniform Size fallback
            _columnWidths = Enumerable.Repeat(0f, Columns).ToArray();
            _rowHeights   = Enumerable.Repeat(0f, Rows).ToArray();

            if (sizes == null || sizes.Length == 0) return;

            var n = Math.Min(sizes.Length, Rows * Columns);
            for (int i = 0; i < n; i++)
            {
                var col = i % Columns;
                var row = i / Columns;

                var r = sizes[i];
                if (r.x  > _columnWidths[col]) _columnWidths[col] = r.x;
                if (r.y > _rowHeights[row])   _rowHeights[row]   = r.y;
            }
        }

        public Bounds Bounds
        {
            get
            {
                var xSize = _columnWidths.Sum() + Spacing.x * Mathf.Max(0, Columns - 1);
                var ySize = _rowHeights.Sum() + Spacing.y * Mathf.Max(0, Rows - 1);
                return new Bounds(new Vector3(0, 0, Size.z / 2f), new Vector3(xSize, ySize, Size.z));
            }
        }

        public SpatialCell GetCell(int index)
        {
            var x = index % Columns;
            var y = index / Columns;

            float cellWidth, cellHeight, xpos, ypos;

            // Sizes from per-column/per-row maxes
            cellWidth  = _columnWidths[x];
            cellHeight = _rowHeights[y];

            // Offsets = sum of previous widths/heights + spacing
            xpos = 0f;
            for (int c = 0; c < x; c++) xpos += _columnWidths[c] + Spacing.x;

            ypos = 0f;
            for (int r = 0; r < y; r++) ypos += _rowHeights[r] + Spacing.y;

            var cellSize   = new Vector3(cellWidth, cellHeight, Size.z);
            var cellBounds = new Bounds(cellSize / 2f, cellSize);

            var cellMatrix = Matrix4x4.TRS(Bounds.min + new Vector3(xpos, ypos, 0f), Quaternion.identity, Vector3.one);
            return new(cellMatrix, cellBounds);
        }
    }
}
