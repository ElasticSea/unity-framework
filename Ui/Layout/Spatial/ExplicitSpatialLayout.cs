using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class ExplicitSpatialLayout : MonoBehaviour, ISpatialLayout
    {
        [SerializeField] private Bounds[] bounds;

        public int Count => bounds.Length;
        public Bounds Bounds => bounds.Encapsulate();

        public SpatialCell GetCell(int index)
        {
            return new SpatialCell(Matrix4x4.identity, bounds[index]);
        }
    }
}