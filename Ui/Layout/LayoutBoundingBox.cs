using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Util;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public class LayoutBoundingBox : MonoBehaviour
    {
        [SerializeField] private BoundingBox boundingBox;
        [SerializeField] private LayoutGroup layoutGroup;

        private void Start()
        {
            layoutGroup.Bind(rect => boundingBox.LocalBounds = new Bounds(rect.center, rect.size));
        }
    }
}