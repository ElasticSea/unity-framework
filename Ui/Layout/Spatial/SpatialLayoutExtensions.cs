using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public static class SpatialLayoutExtensions
    {
        public static int Count(this ISpatialGrid spatialGrid) => spatialGrid.Columns * spatialGrid.Rows;

        public static SpatialCell GetCell(this ISpatialGrid grid, int row, int column)
        {
            return grid.GetCell(column - 1 + row * grid.Columns);
        }

        public static SpatialCell[] GetCells(this ISpatialLayout grid)
        {
            var count = grid.Count;
            var cells = new SpatialCell[count];
            for (var i = 0; i < count; i++)
            {
                cells[i] = grid.GetCell(i);
            }

            return cells;
        }

        public static (Matrix4x4 localToWorld, Bounds bounds) GetWorldCell(this ISpatialGrid grid, Transform gridTransform, int row, int column)
        {
            var cell = grid.GetCell(row, column);
            var localToWorld = gridTransform.localToWorldMatrix * cell.cellToLocal;
            return (localToWorld, cell.localBounds);
        }

        public static Pose GetPoseInCellLocal(this ISpatialLayout grid, Bounds bounds, int cellIndex,
            Align horizontal = Align.Center, Align vertical = Align.Center, Align depth = Align.Center)
        {
            var cell = grid.GetCell(cellIndex);
            return cell.GetPoseInCellLocal(bounds, horizontal, vertical, depth);
        }

        public static Pose GetPoseInCellLocal(this SpatialCell spatialCell, Bounds bounds,
            Align horizontal = Align.Center, Align vertical = Align.Center, Align depth = Align.Center)
        {
            var (position, scale) = BoundsAlignUtils.Align(bounds, spatialCell.cellToLocal, spatialCell.localBounds, horizontal, vertical, depth, ScaleAlignmentType.None);
            return new Pose(position, spatialCell.cellToLocal.rotation);
        }
    }
}