using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    public class AlignRelativeTo : MonoBehaviour
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _anchor;
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _follower;
        [SerializeField] private Align horizontal = Align.Center;
        [SerializeField] private Align vertical = Align.Center;
        [SerializeField] private float borderOffset;

        private void OnEnable()
        {
            ((ILayoutComponent)_anchor).OnRectChanged += Refresh;
            ((ILayoutComponent)_follower).OnRectChanged += Refresh;
        }
        
        private void OnDisable()
        {
            ((ILayoutComponent)_anchor).OnRectChanged -= Refresh;
            ((ILayoutComponent)_follower).OnRectChanged -= Refresh;
        }

        private void Refresh()
        {
            var anchor = (ILayoutComponent)_anchor;
            var follower = (ILayoutComponent)_follower;
            var anchorTransform = _anchor.transform;
            var followerTransform = _follower.transform;

            var offset = anchor.Rect.AlignInsideRect(follower.Rect, horizontal, vertical);

            offset.x += GetBorderOffset(horizontal);
            offset.y += GetBorderOffset(vertical);
            
            followerTransform.localPosition = offset;
        }

        private float GetBorderOffset(Align align)
        {
            switch (align)
            {
                case Align.Start:
                    return borderOffset;
                case Align.End:
                    return -borderOffset;
                case Align.BeforeStart:
                    return -borderOffset;
                case Align.AfterEnd:
                    return borderOffset;
                default:
                    return 0;
            }
        }
    }
}