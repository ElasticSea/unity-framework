using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ui.Components.Validators
{
    public class IsUniqueName : Validator
    {
        [SerializeField] private List<string> usedNames;

        public List<string> UsedNames
        {
            get { return usedNames; }
            set { usedNames = value; }
        }

        public override ValidationResult Validate(string arg0)
        {
            var conflit = usedNames.Any(used => string.Equals(used, arg0, StringComparison.CurrentCultureIgnoreCase));
            
            return new ValidationResult{IsValid = conflit == false};
        }
    }
}