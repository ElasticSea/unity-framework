using System;

namespace _Framework.Scripts.Ui.Components.Validators
{
    public class FuncValidator : Validator
    {
        public Func<string, bool> Func;

        public override ValidationResult Validate(string str)
        {
            return new ValidationResult{IsValid = Func(str)};
        }
    }
}