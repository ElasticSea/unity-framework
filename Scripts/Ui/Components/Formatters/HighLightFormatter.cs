using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Framework.Scripts.Ui.Components.Formatters
{
    public class HighLightFormatter : SelectFormatter
    {
        [SerializeField] protected Graphic[] foreground;
        [SerializeField] protected Graphic[] background;

        public Color OnBackgroundColor = Color.white;
        public Color OnForegroundColor = Color.black;
        public Color OffBackgroundColor = Color.black.SetAlpha(200 / 256f);
        public Color OffForegroundColor = Color.white;

        public bool OnDisableBackgroundColor;
        public bool OnDisableForegroundColor;
        public bool OffDisableBackgroundColor;
        public bool OffDisableForegroundColor;

        public override void OnSelected(bool selected)
        {
            if (selected)
            {
                foreach (var b in background)
                {
                    b.enabled = OnDisableBackgroundColor == false;
                    b.color = OnBackgroundColor;
                }

                foreach (var f in foreground)
                {
                    f.enabled = OnDisableForegroundColor == false;
                    f.color = OnForegroundColor;
                }
            }
            else
            {
                foreach (var b in background)
                {
                    b.enabled = OffDisableBackgroundColor == false;
                    b.color = OffBackgroundColor;
                }

                foreach (var f in foreground)
                {
                    f.enabled = OffDisableForegroundColor == false;
                    f.color = OffForegroundColor;
                }
            }
        }
    }
}