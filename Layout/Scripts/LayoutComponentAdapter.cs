using System;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class LayoutComponentAdapter : MonoBehaviour, ILayoutComponent
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _targetLayout;

        [SerializeField] private float paddingLeft;
        [SerializeField] private float paddingRight;
        [SerializeField] private float paddingTop;
        [SerializeField] private float paddingBottom;
        [SerializeField] private float minWidth = -1;
        [SerializeField] private float minHeight = -1;
        
        private ILayoutComponent targetLayout => _targetLayout as ILayoutComponent;
        
        private Rect rect;
        
        private void OnEnable()
        {
            targetLayout.OnRectChanged += LayoutChanged;
            LayoutChanged();
        }

        private void OnDisable()
        {
            targetLayout.OnRectChanged += LayoutChanged;
        }

        private void LayoutChanged()
        {
            var r = targetLayout.Rect; 
            var c = r.center; 

            float w = r.width + paddingLeft + paddingRight;
            float h = r.height + paddingTop + paddingBottom;

            if (minWidth >= 0) w = Mathf.Max(minWidth, w);
            if (minHeight >= 0) h = Mathf.Max(minHeight, h);

            r.size = new Vector2(w, h);
            r.center = c;

            rect = r;
            
            OnRectChanged?.Invoke();
        }

        private void OnValidate()
        {
            if (targetLayout != null)
            {
                LayoutChanged();
            }
        }

        public Rect Rect => rect;
        public event Action OnRectChanged;
    }
}