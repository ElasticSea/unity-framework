using ElasticSea.Framework.Ui.Layout.Alignment;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public class AlignLayoutComponent : MonoBehaviour
    {
        [CustomObjectPicker(typeof(ILayoutComponent))] [SerializeField]
        private Component _layoutComponent;

        private ILayoutComponent layoutComponent => (ILayoutComponent)_layoutComponent;
        [SerializeField] private Align horizontal = Align.Center;
        [SerializeField] private Align vertical = Align.Center;

        private void Awake()
        {
            layoutComponent.OnRectChanged += () => AlignLayout();
            AlignLayout();
        }

        private void AlignLayout()
        {
            var rect = layoutComponent.Rect;
            var horizontalDelta = horizontal.GetAlignDelta();
            var verticalDelta = vertical.GetAlignDelta();
            var x = -Mathf.Lerp(rect.xMin, rect.xMax, horizontalDelta);
            var y = -Mathf.Lerp(rect.yMin, rect.yMax, verticalDelta);
            transform.localPosition = new Vector3(x, y, 0);
        }

        private void OnValidate()
        {
            AlignLayout();
        }
    }
}