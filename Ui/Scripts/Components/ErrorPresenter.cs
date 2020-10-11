using UnityEngine;

namespace Ui.Components
{
    public abstract class ErrorPresenter : MonoBehaviour
    {
        public abstract void PresentError(ValidationResult result);
    }
}