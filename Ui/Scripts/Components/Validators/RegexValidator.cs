using System.Text.RegularExpressions;
using UnityEngine;

namespace Ui.Components.Validators
{
    public class RegexValidator : Validator
    {
        [SerializeField] private string regex;
        private Regex rgx;

        public string Regex
        {
            get => regex;
            set
            {
                regex = value;
                rgx = new Regex(regex);
            }
        }

        private void Awake()
        {
            rgx = new Regex(regex);
        }

        public override ValidationResult Validate(string arg0)
        {
            return new ValidationResult {IsValid = rgx.IsMatch(arg0)};
        }
    }
}