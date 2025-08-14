using System;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class StaticLayoutComponent : MonoBehaviour, ILayoutComponent
    {
        [SerializeField] private Rect rect = new(0, 0, 1, 1);

        public Rect Rect
        {
            get => rect;
            set => rect = value;
        }

        public event Action OnRectChanged;
        
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(rect.center, rect.size);
        }
    }
}