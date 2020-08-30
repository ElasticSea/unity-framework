using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _Framework.Scripts.Ui.Components.Formatters
{
    public class NavHighlightFormatter : SelectFormatter
    {
        [SerializeField] private Graphic[] graphics;

        private Dictionary<Graphic, Color> savedColors;

        public override void OnSelected(bool selected)
        {
            if (savedColors == null)
            {
                savedColors = graphics.ToDictionary(g => g, g => g.color);
            }

            foreach (var g in graphics)
            {
                var color = savedColors[g];
                var threashold = (color.r + color.g + color.b) / 3;
                var offset = threashold > .5f ? -.2f : .2f;
                var newColor = new Color(color.r + offset, color.g+ offset, color.b+ offset, color.a);
                g.color = selected ? newColor : color;
            }
        }
    }
}