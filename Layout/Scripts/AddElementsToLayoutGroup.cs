using System.Linq;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class AddElementsToLayoutGroup : MonoBehaviour
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component[] _elements;
        [SerializeField] private LayoutGroup layoutGroup;

        private void Awake()
        {
            var elements = _elements
                .Select(c => c as ILayoutComponent)
                .Where(l => l != null)
                .ToArray();

            layoutGroup.AddElements(elements);
        }
    }
}