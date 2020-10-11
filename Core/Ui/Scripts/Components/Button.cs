using System;
using Core.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Ui.Components
{
     public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IClickable
    {
        [SerializeField] private Color backgroundOff = Color.white;
        [SerializeField] private Color foregroundOff = Color.black;
        [SerializeField] private Color foregroundOn = Color.white;
        [SerializeField] private Color backgroundOn = Color.black.SetAlpha(200 / 256f);
        [SerializeField] private Graphic background;
        [SerializeField] private Text text;
        [SerializeField] private Text icon;

        public string Text
        {
            get { return text.text; }
            set
            {
                text.text = value;
                text.gameObject.SetActive(text.text.IsNullOrEmpty() == false);
            }
        }

        public string Icon
        {
            get { return icon.text; }
            set
            {
                icon.text = value;
                icon.gameObject.SetActive(icon.text.IsNullOrEmpty() == false);
            }
        }

        public Color BackgroundOff
        {
            get { return backgroundOff; }
            set
            {
                backgroundOff = value;
                Released();
            }
        }

        public Color ForegroundOff
        {
            get { return foregroundOff; }
            set
            {
                foregroundOff = value;
                Released();
            }
        }

        public Color ForegroundOn
        {
            get { return foregroundOn; }
            set
            {
                foregroundOn = value;
                Released();
            }
        }

        public Color BackgroundOn
        {
            get { return backgroundOn; }
            set
            {
                backgroundOn = value;
                Released();
            }
        }

        private void Awake()
        {
            Released();
        }

        public void Pressed()
        {
            Recolor(ForegroundOn, BackgroundOn);
        }

        public void Released()
        {
            Recolor(ForegroundOff, BackgroundOff);
        }

        private void Recolor(Color foreground, Color background)
        {
            this.background.color = background;
            if (text) text.color = foreground;
            if (icon) icon.color = foreground;
        }

        public void OnPointerDown(PointerEventData eventData) => Pressed();
        public void OnPointerUp(PointerEventData eventData) => Released();
        public void OnPointerClick(PointerEventData eventData) => OnClick();
        public event Action OnClick = () => {};
    }
}