using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Placement
{
    public class FlatPlacementGrid : MonoBehaviour, IPlacementGrid
    {
        [SerializeField] private int rows;
        [SerializeField] private int columns;
        [SerializeField] private Vector3 size;
        [SerializeField] private Vector2 spacing;
        [SerializeField] private Align horizontalAlign = Align.Center;
        [SerializeField] private Align verticalAlign = Align.Center;

        public int Rows
        {
            get => rows;
            set => rows = value;
        }

        public int Columns
        {
            get => columns;
            set => columns = value;
        }

        public Vector2 Spacing
        {
            get => spacing;
            set => spacing = value;
        }

        public Vector3 Size
        {
            get => size;
        }

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
            set => throw new System.NotImplementedException();
        }

        public (Matrix4x4 cellToLocal, Bounds bounds) GetCell(int index)
        {
            var x = index % Columns;
            var y = index / Columns;
            
            var xpos = x * (size.x + spacing.x);
            var ypos = y * (size.y + spacing.y);
            
            var cellBounds = new Bounds(size / 2, size);

            var cellMatrix = Matrix4x4.TRS(Bounds.min + new Vector3(xpos, ypos, 0), Quaternion.identity, Vector3.one);

            return (cellMatrix, cellBounds);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size * 1.01f);
            
            PlacementGridUtils.DrawCells(this, Count, transform);
        }
    }
}