using System;
using Core.Extensions;
using Core.Ui.Binding;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Ui.Components
{
    public class SlideToggle : MonoBehaviour, IUiBinding<bool>, IClickable
    {
        public RectTransform Button;

        private bool selected;
        private float minXpos;
        private float maxXpos;
        private Tweener transition;
        private RectTransform rectTransform;

        public bool Value
        {
            get { return selected; }

            set { UpdateValue(value); }
        }

        public event Action OnClick = () => { };

        private void Select(bool animate)
        {
            if (selected)
            {
                SetOn(animate);
            }
            else
            {
                SetOff(animate);
            }
        }

        public void UpdateValue(bool value)
        {
            selected = value;
            Select(true);

            OnValueChanged.Invoke(selected);
        }

        public event Action<bool> OnValueChanged = b => { };

        private void SetOn(bool animate)
        {
            transition.PlayForward();
            if (animate == false) transition.Complete();
        }

        private void SetOff(bool animate)
        {
            transition.PlayBackwards();
            if (animate == false) transition.Goto(0, true);
        }

        private void Awake()
        {
            // IPointerClickHandler does not seem to work on this object 2018.3
            gameObject.GetOrAddComponent<EventTrigger>().Click(arg0 =>
            {
                OnClick();
                Value = !Value;
            });
            rectTransform = GetComponent<RectTransform>();

            OnRectTransformDimensionsChange();
        }

        void OnRectTransformDimensionsChange()
        {
            // Can get called before Awake
            if (rectTransform == null) return;

            var size = Button.GetHeight();
            var padding = (rectTransform.GetHeight() - size) / 2f;

            minXpos = padding;
            maxXpos = rectTransform.GetWidth() - size - padding;

            Button.anchoredPosition = Button.anchoredPosition.SetX(minXpos);
            transition?.Kill();
            transition = Button.DOAnchorPosX(maxXpos, .2f)
                .SetAutoKill(false)
                .SetUpdate(true)
                .Pause();

            Select(false);
        }
    }
}