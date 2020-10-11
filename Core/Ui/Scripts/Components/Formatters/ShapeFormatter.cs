using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components.Formatters
{
    public class ShapeFormatter : ValueFormatter
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite first;
        [SerializeField] private Sprite center;
        [SerializeField] private Sprite last;
        [SerializeField] private Sprite both;
        
        public override void OnValueChanged(object o)
        {
            image.sprite = GetShape();
        }

        private Sprite GetShape()
        {
            var parent = transform.parent;
            var isFirst = parent.GetChild(0) == transform;
            var isLast = parent.GetChild(parent.childCount - 1) == transform;

            if (isFirst && isLast) return both;
            if (isFirst) return first;
            if (isLast) return last;
            return center;
        }
    }
}