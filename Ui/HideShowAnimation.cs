using DG.Tweening;
using ElasticSea.Framework.Util;

namespace ElasticSea.Framework.Ui
{
    internal class HideShowAnimation : IHideShowAnim
    {
        private readonly Tween tween;

        public HideShowAnimation(Tween tween)
        {
            this.tween = tween.SetAutoKill(false).Pause();
        }
        
        public void Show(bool animate = true)
        {
            tween.PlayForward();
            if (animate == false) tween.Complete();
        }

        public void Hide(bool animate = true)
        {
            tween.PlayBackwards();
            if (animate == false) tween.Goto(0, true);
        }
    }
}