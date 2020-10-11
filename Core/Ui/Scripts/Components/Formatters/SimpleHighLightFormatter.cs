using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components.Formatters
{
    public class SimpleHighLightFormatter : SelectFormatter
    {
        [SerializeField] private Graphic[] graphics;
        [SerializeField] private Color on = Color.white;
        [SerializeField] private Color off = Color.black.SetAlpha(200 / 256f);

        public override void OnSelected(bool selected)
        {
            foreach (var g in graphics)
                g.color = selected ? on : off;
        }
    }
}