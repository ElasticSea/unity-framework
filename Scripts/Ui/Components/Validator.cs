using UnityEngine;

namespace _Framework.Scripts.Ui.Components
{
    public abstract class Validator: MonoBehaviour
    {
        public abstract ValidationResult Validate(string arg0);
    }
}