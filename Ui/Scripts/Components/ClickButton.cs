using System;
using Core.Extensions;
using Ui.Components.Formatters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ui.Components
{
    public class ClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IClickable
    {
        [SerializeField] private SelectFormatter[] selectFormatters;

        [SerializeField] private Text text;
        [SerializeField] private Text icon;

        private bool selected;

        public bool Selected
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    selected = value;
                    foreach (var formatter in selectFormatters)
                    {
                        formatter.OnSelected(Selected);
                    }
                }
            }
        }

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

        private void Awake()
        {
            Selected = false;
        }

        public void Pressed()
        {
            Selected = true;
        }

        public void Released()
        {
            Selected = false;
        }

        public void OnPointerDown(PointerEventData eventData) => Pressed();
        public void OnPointerUp(PointerEventData eventData) => Released();
        public void OnPointerClick(PointerEventData eventData) => OnClick();
        public event Action OnClick = () => { };
    }
}