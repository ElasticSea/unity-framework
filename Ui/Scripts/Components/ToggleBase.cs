using System;
using UnityEngine;

namespace Ui.Components
{
    public class ToggleBase : MonoBehaviour, IToggle
    {
        public virtual bool Selected { get; set; }
        public event Action OnClick = () => { };

        protected void TriggerClickEvent() => OnClick();
    }
}