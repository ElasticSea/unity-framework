using System;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public interface ILayoutComponent
    {
        Rect Rect { get; }
        
        event Action OnRectChanged;
    }
}