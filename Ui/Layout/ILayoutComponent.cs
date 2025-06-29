using System;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public interface ILayoutComponent
    {
        Rect Rect { get; }
        event Action OnRectChanged;
    }
}