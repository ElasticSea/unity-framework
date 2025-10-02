using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public static class SpatialLayoutUtils
    {
        public static void DrawLayout(ISpatialLayout spatialLayout, int count, Transform transform)
        {
            var bounds = spatialLayout.Bounds;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            
            for (int i = 0; i < count; i++)
            {
                var cell = spatialLayout.GetCell(i);
                var cellBounds = cell.localBounds;

                Gizmos.color = Color.Lerp(Color.red, Color.blue, i / (count - 1f));
                Gizmos.matrix = transform.localToWorldMatrix * cell.cellToLocal;
                Gizmos.DrawWireCube(cell.localBounds.center, cell.localBounds.size);

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