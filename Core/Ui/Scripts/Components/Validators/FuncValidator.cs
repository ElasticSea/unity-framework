using System;

namespace Core.Ui.Components.Validators
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