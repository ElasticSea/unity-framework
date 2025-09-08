using System;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;

namespace ElasticSea.Framework.Layout
{
    /// Align element follower to anchor when source rect changes
    public class AlignRelativeTo2 : MonoBehaviour
    {
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _source;
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _horizontalAlign;
        [SerializeField, CustomObjectPicker(typeof(ILayoutComponent))] private Component _verticalAlign;
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
                var anchorRect = ((ILayoutComponent)_source).Rect;
                var followerRect = ((ILayoutComponent)_follower).Rect;
                var anchor = _source.transform;
                var follower = _follower.transform;
            
                var offsetX = ((ILayoutComponent)_horizontalAlign).Rect.AlignInsideRect(followerRect, horizontal, Align.Center, borderOffset);
                var offsetY = ((ILayoutComponent)_verticalAlign).Rect.AlignInsideRect(followerRect, Align.Center, vertical, borderOffset);
                var offset = new Vector2(offsetX.x, offsetY.y);
                var localPos = new Vector2(_horizontalAlign.transform.localPosition.x, _verticalAlign.transform.localPosition.y);
                follower.localPosition = offset + anchor.localPosition.FromXY() + localPos;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}