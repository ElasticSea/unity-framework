using UnityEngine;

namespace Core.Ui.Components
{
    public abstract class Validator: MonoBehaviour
    {
        public abstract ValidationResult Validate(string arg0);
    }
}