using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public interface IUniformSpatialLayout : ISpatialLayout
    {
        Vector3 Size { get; set; }
    }
}