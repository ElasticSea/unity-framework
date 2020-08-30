namespace _Framework.Scripts.Ui.Components.Validators
{
    public class IsNumValidator : Validator
    {
        public override ValidationResult Validate(string arg0)
        {
            return new ValidationResult{IsValid = double.TryParse(arg0, out var resultNum)};
        }
    }
}