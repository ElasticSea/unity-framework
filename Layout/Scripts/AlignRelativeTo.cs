using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    /// Align element follower to anchor when source rect changes
    public class AlignRelativeTo : MonoBehaviour
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _anchor;
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _source;
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _follower;
        [SerializeField] private Align horizontal = Align.Center;
        [SerializeField] private Align vertical = Align.Center;
        [SerializeField] private float borderOffset;

        private void OnEnable()
        {
            ((ILayoutComponent)_source).OnRectChanged += Refresh;
            ((ILayoutComponent)_follower).OnRectChanged += Refresh;
        }
        
        private void OnDisable()
        {
            ((ILayoutComponent)_source).OnRectChanged -= Refresh;
            ((ILayoutComponent)_follower).OnRectChanged -= Refresh;
        }

        private void Refresh()
        {
            try
            {
                var anchorRect = ((ILayoutComponent)_anchor).Rect;
                var followerRect = ((ILayoutComponent)_follower).Rect;
                var anchor = _anchor.transform;
                var follower = _follower.transform;
            
                if (anchor.parent != follower.parent)
                {
                    throw new Exception("Anchor and follower must be in the same parent");
                }
            
                var offset = anchorRect.AlignInsideRect(followerRect, horizontal, vertical, borderOffset);
                follower.localPosition = offset + anchor.localPosition.FromXY();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}