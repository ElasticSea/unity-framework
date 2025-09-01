using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public struct SpatialCell
    {
        public Matrix4x4 cellToLocal;
        public Bounds localBounds;

        public SpatialCell(Matrix4x4 cellToLocalMatrix, Bounds cellLocalBounds)
        {
            cellToLocal = cellToLocalMatrix;
            localBounds = cellLocalBounds;
        }
    }
}