using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class SpatialLayoutGizmos : MonoBehaviour
    {
        [SerializeField, CustomObjectPicker(typeof(ISpatialLayout))] private Component _spatialLayout;
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
            var spatialLayout = (ISpatialLayout)_spatialLayout;
            var bounds = spatialLayout.Bounds;
            var count = spatialLayout.Count;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            for (int i = 0; i < count; i++)
            {
                var cell = spatialLayout.GetCell(i);
                Gizmos.matrix = transform.localToWorldMatrix * cell.cellToLocal;
                Gizmos.color = Color.red;
                var radius = cell.localBounds.size.FromXY().Min() / 2;
                GizmoUtils.DrawCircle(cell.localBounds.center, Vector3.back, radius);
            }
        }
    }
}