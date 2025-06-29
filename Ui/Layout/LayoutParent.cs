using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public class LayoutParent : MonoBehaviour
    {
        [SerializeField] private Align horizontalAlign = Align.Center;
        [SerializeField] private Align verticalAlign = Align.Center;
        [SerializeField] private Rect rect;
        [SerializeField] private LayoutGroup child;

        private void Awake()
        {
            child.OnRectChanged += () => Refresh();
            Refresh();
        }

        private void Refresh()
        {
            LayoutUtils.Align(child.transform, rect, child.Rect, horizontalAlign, verticalAlign);
        }

        private void OnValidate()
        {
            Refresh();
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(rect.center, rect.size);
        }
    }
}