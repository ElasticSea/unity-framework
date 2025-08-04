using System.Linq;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class AddChildrenToLayoutGroup : MonoBehaviour
    {
        [SerializeField] private LayoutGroup layoutGroup;

        private void Awake()
        {
            var layoutChildren = transform.Children()
                .Select(c => c.GetComponent<ILayoutComponent>())
                .Where(l => l != null)
                .ToArray();

            layoutGroup.AddElements(layoutChildren);
        }
    }
}