using System;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class LayoutSpace : MonoBehaviour, ILayoutComponent
    {
        [SerializeField] private float size;

        public Rect Rect => Rect.MinMaxRect(0, 0, size, size);
        public event Action OnRectChanged;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                OnRectChanged?.Invoke();   
            }
        }
    }
}