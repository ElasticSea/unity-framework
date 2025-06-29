using System;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public class Space : MonoBehaviour, ILayoutComponent
    {
        [SerializeField] private float size;

        public Rect Rect => Rect.MinMaxRect(0, 0, size, size);
        public event Action OnRectChanged;
    }
}