using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Ui.Layout.Placement;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class FlatSpatialGrid : ISpatialGrid
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Vector2 Spacing { get; set; }
        public Vector3 Size { get; set; }
        public int Count => this.Count();

        public Bounds Bounds
        {
            get
            {
                var xSize = Size.x * Columns + Spacing.x * (Columns - 1);
                var ySize = Size.y * Rows + Spacing.y * (Rows - 1);
                var bounds = new Bounds(new Vector3(0, 0, Size.z / 2), new Vector3(xSize, ySize, Size.z));
                return bounds;
            }
        }

        public SpatialCell GetCell(int index)
        {
            var x = index % Columns;
            var y = index / Columns;

            var xpos = x * (Size.x + Spacing.x);
            var ypos = y * (Size.y + Spacing.y);

            var cellBounds = new Bounds(Size / 2, Size);

            var cellMatrix = Matrix4x4.TRS(Bounds.min + new Vector3(xpos, ypos, 0), Quaternion.identity, Vector3.one);

            return new(cellMatrix, cellBounds);
        }
    }
}