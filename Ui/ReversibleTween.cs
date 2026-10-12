using System;
using DG.Tweening;
using UnityEngine;

namespace ElasticSea.Framework.Ui
{
    /// <summary>Plays a tween forwards or backwards from its current progress.</summary>
    /// <remarks>The tween is killed when its owner is destroyed or this wrapper is disposed.</remarks>
    public sealed class ReversibleTween : IDisposable
    {
        private readonly Tween tween;

        public ReversibleTween(Tween tween, GameObject owner)
        {
            if (tween == null) throw new ArgumentNullException(nameof(tween));
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            this.tween = tween.SetAutoKill(false).SetRecyclable(false).Pause()
                .SetLink(owner, LinkBehaviour.KillOnDestroy);
        }

        public void Forward(bool animate = true)
        {
            if (!tween.IsActive()) return;
            tween.PlayForward();
            if (animate == false) tween.Complete();
        }

        public void Backward(bool animate = true)
        {
            if (!tween.IsActive()) return;
            tween.PlayBackwards();
            if (animate == false) tween.Goto(0, true);
        }

        public void Dispose() => tween.Kill();
    }
}
