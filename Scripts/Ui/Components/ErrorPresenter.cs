using UnityEngine;

namespace _Framework.Scripts.Ui.Components
{
    public abstract class ErrorPresenter : MonoBehaviour
    {
        public abstract void PresentError(ValidationResult result);
    }
}