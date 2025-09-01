using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public interface ISpatialLayout
    {
        int Count { get; }
        Bounds Bounds { get; }
        SpatialCell GetCell(int index);
    }
}