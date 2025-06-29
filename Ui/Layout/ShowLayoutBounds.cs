using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public class ShowLayoutBounds : MonoBehaviour
    {
        [CustomObjectPicker(typeof(ILayoutComponent))]
        [SerializeField] private Component _layoutComponent;
        public ILayoutComponent LayoutComponent
        {
            get => _layoutComponent as ILayoutComponent;
            set => _layoutComponent = (Component)value;
        }

        private void OnDrawGizmosSelected()
        {
            var rect = LayoutComponent.Rect;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(rect.center, rect.size);
        }
    }
}