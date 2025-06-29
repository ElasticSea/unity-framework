using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Placement
{
    public class PlacementGridUtils
    {
        public static void DrawCells(IPlacement placementGrid, int count, Transform transform)
        {
            for (int i = 0; i < count; i++)
            {
                var cell = placementGrid.GetCell(i);
                var cellBounds = cell.bounds;

                Gizmos.color = Color.Lerp(Color.red, Color.blue, i / (count - 1f));
                Gizmos.matrix = transform.localToWorldMatrix * cell.cellToLocal;
                Gizmos.DrawWireCube(cell.bounds.center, cell.bounds.size);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(cellBounds.center, cellBounds.center + new Vector3(cellBounds.size.x / 4, 0, 0));

                Gizmos.color = Color.green;
                Gizmos.DrawLine(cellBounds.center, cellBounds.center + new Vector3(0, cellBounds.size.y / 4, 0));

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(cellBounds.center, cellBounds.center + new Vector3(0, 0, cellBounds.size.z / 4));
            }
        }
    }
}