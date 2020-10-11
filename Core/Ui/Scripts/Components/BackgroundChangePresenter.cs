using UnityEngine;
using UnityEngine.UI;

namespace Core.Ui.Components
{
    public class BackgroundChangePresenter : ErrorPresenter
    {
        [SerializeField] private Image background;
        [SerializeField] private Color validColor = Color.clear;
        [SerializeField] private Color invalidColor = Color.red;
        
        public override void PresentError(ValidationResult result)
        {
            background.color = result.IsValid ? validColor : invalidColor;
        }
    }
}