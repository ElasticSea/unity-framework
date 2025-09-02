using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class LayoutWrapper : MonoBehaviour, ILayoutComponent
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _layout;

        private void OnEnable()
        {
            ((ILayoutComponent)_layout).OnRectChanged += Callback;
        }

        private void OnDisable()
        {
            ((ILayoutComponent)_layout).OnRectChanged -= Callback;
        }

        private void Callback()
        {
            OnRectChanged?.Invoke();
        }

        public Rect Rect => _layout.transform.TransformRect(transform, ((ILayoutComponent)_layout).Rect);
        public event Action OnRectChanged;
    }
}