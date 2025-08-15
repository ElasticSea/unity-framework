using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Placement
{
    public static class PlacementGridExtensions
    {
        public static int Count(this IPlacementGrid placementGrid) => placementGrid.Columns * placementGrid.Rows;

        public static (Matrix4x4 cellToLocal, Bounds bounds) GetCell(this IPlacementGrid grid, int row, int column)
        {
            return grid.GetCell(column - 1 + row * grid.Columns);
        }

        public static (Matrix4x4 cellToLocal, Bounds bounds)[] GetCells(this IPlacementGrid grid)
        {
            var count = grid.Count;
            var cells = new (Matrix4x4 cellToLocal, Bounds bounds)[count];
            for (var i = 0; i < count; i++)
            {
                cells[i] = grid.GetCell(i);
            }

            return cells;
        }

        public static (Matrix4x4 localToWorld, Bounds bounds) GetWorldCell(this IPlacementGrid grid, Transform gridTransform, int row, int column)
        {
            var (cellToLocal, bounds) = grid.GetCell(row, column);
            var localToWorld = gridTransform.localToWorldMatrix * cellToLocal;
            return (localToWorld, bounds);
        }

        public static Pose GetPoseInCellLocal(this IPlacementGrid grid, Bounds bounds, int cellIndex,
            Align horizontal = Align.Center, Align vertical = Align.Center, Align depth = Align.Center)
        {
            var cell = grid.GetCell(cellIndex);
            var (position, scale) = BoundsAlignUtils.Align(bounds, cell.cellToLocal, cell.bounds, horizontal, vertical, depth, ScaleAlignmentType.None);
            return new Pose(position, cell.cellToLocal.rotation);
        }
    }
}