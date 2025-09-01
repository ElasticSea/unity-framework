using System;
using ElasticSea.Framework.Util;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class RectSpatialLayout : ISpatialLayout
    {
        private readonly Rect[] rects;
        private readonly Rect rectBounds;

        public int Count => rects.Length;
        public Bounds Bounds => new(rectBounds.center, rectBounds.size);

        public RectSpatialLayout(Rect[] rects)
        {
            this.rects = rects;
            this.rectBounds = GetRectBounds();
        }

        public SpatialCell GetCell(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var rect = rects[index];

            // place the cell at the rect's origin (top-left per Unity Rect convention)
            var translate = new Vector2(rect.xMin, rect.yMin);

            // per-cell bounds are local to the cell, centered at half-size
            var cellSize = new Vector2(rect.width, rect.height);
            var cellCenter = new Vector2(rect.width * 0.5f, rect.height * 0.5f);
            var cellBounds = new Bounds(cellCenter, cellSize);

            var cellToLocal = Matrix4x4.TRS(translate, Quaternion.identity, Vector3.one);
            return new SpatialCell(cellToLocal, cellBounds);
        }

        private Rect GetRectBounds()
        {
            if (rects.Length == 0)
                return new Rect();

            var minX = float.PositiveInfinity;
            var minY = float.PositiveInfinity;
            var maxX = float.NegativeInfinity;
            var maxY = float.NegativeInfinity;

            for (int i = 0; i < rects.Length; i++)
            {
                var r = rects[i];
                if (r.xMin < minX) minX = r.xMin;
                if (r.yMin < minY) minY = r.yMin;
                if (r.xMax > maxX) maxX = r.xMax;
                if (r.yMax > maxY) maxY = r.yMax;
            }

            return Utils.MinMaxRect(minX, minY, maxX, maxY);
        }
    }
}