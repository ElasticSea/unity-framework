using System;
using UnityEngine;

namespace Core.Ui.Components
{
    public class Toggle : MonoBehaviour, IToggle
    {
        public virtual bool Selected { get; set; }
        public event Action OnClick = () => { };

        protected void TriggerClickEvent() => OnClick();
    }
}