using ElasticSea.Framework.Ui.Layout.Alignment;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Layout
{
    public class LayoutUtils
    {
        public static void AlignAboveTopCenter(Transform obj, Rect bounds, Rect toAlign, float offset = 0)
        {
            Align(obj, bounds, toAlign, Alignment.Align.Center, Alignment.Align.AfterEnd);
            obj.localPosition += Vector3.up * offset;
        }
        
        public static void AlignBelowBottomCenter(Transform obj, Rect bounds, Rect toAlign, float offset = 0)
        {
            Align(obj, bounds, toAlign, Alignment.Align.Center, Alignment.Align.BeforeStart);
            obj.localPosition += Vector3.down * offset;
        }
        
        public static void Align(Transform obj, Rect bounds, Rect toAlign, Align horizontal, Align vertical)
        {
            var objXYOffset = bounds.AlignInsideRect(toAlign, horizontal, vertical);
            var buttonsPosition = new Vector3(objXYOffset.x, objXYOffset.y, 0);
            obj.localPosition = buttonsPosition;
        }
    }
}