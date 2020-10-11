using UnityEngine;

namespace Ui.Components
{
    public abstract class Validator: MonoBehaviour
    {
        public abstract ValidationResult Validate(string arg0);
    }
}