using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using _Framework.Scripts.Extensions;
using _Framework.Scripts.Ui.Binding;

namespace _Framework.Scripts.Ui.Components
{
    public class SlideToggle : MonoBehaviour, IUiBinding<bool>
    {
        [FormerlySerializedAs("Button")] [SerializeField] private RectTransform button;

        private bool selected;
        private float minXpos;
        private float maxXpos;
        private Tween transition;
        private RectTransform rectTransform;

        public bool Value
        {
            get => selected;
            set => UpdateValue(value);
        }

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
            rectTransform = GetComponent<RectTransform>();
            GetComponent<EventTrigger>().Click(data =>
            {
                Value = !Value;
            });
        }

        void OnRectTransformDimensionsChange()
        {
            // Can get called before Awake
            if (rectTransform == null) return;

            var size = button.GetHeight();
            var padding = (rectTransform.GetHeight() - size) / 2f;

            minXpos = padding;
            maxXpos = rectTransform.GetWidth() - size - padding;

            button.anchoredPosition = button.anchoredPosition.SetX(minXpos);
            transition?.Kill();
            transition = button.DOAnchorPosX(maxXpos, .2f)
                .SetAutoKill(false)
                .SetUpdate(true)
                .Pause();

            Select(false);
        }
    }
}