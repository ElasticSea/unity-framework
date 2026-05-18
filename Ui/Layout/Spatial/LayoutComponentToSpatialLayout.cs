using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout.Spatial
{
    public class LayoutComponentToSpatialLayout : MonoBehaviour, ISpatialLayout
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _targetLayout;

        private Bounds bounds
        {
            get
            {
                var layout = (ILayoutComponent)_targetLayout;
                var rect = layout.Rect;
                return new Bounds(rect.center, rect.size);
            }
        }

        public int Count => 1;
        public Bounds Bounds => bounds;

        public SpatialCell GetCell(int index)
        {
            return new SpatialCell(Matrix4x4.identity, bounds);
        }
    }
}