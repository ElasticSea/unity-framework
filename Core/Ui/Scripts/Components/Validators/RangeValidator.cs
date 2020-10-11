using System;
using UnityEngine;

namespace Core.Ui.Components.Validators
{
    public class RangeValidator : Validator
    {
        [SerializeField] private RangeType type;
        [SerializeField] private float limit;

        public enum RangeType
        {
            LessThan, GreaterThan, LessThanOrEqualTo, GreaterThanOrEqualTo, EqualTo
        }

        public float Limit
        {
            get { return limit; }
            set { limit = value; }
        }

        public RangeType Type
        {
            get { return type; }
            set { type = value; }
        }

        public override ValidationResult Validate(string arg0)
        {
            if (double.TryParse(arg0, out var resultNum))
            {
                switch (Type)
                {
                    case RangeType.LessThan:
                        return new ValidationResult{IsValid = resultNum < Limit};
                    case RangeType.GreaterThan:
                        return new ValidationResult{IsValid = resultNum > Limit};
                    case RangeType.LessThanOrEqualTo:
                        return new ValidationResult{IsValid = resultNum <= Limit};
                    case RangeType.GreaterThanOrEqualTo:
                        return new ValidationResult{IsValid = resultNum >= Limit};
                    case RangeType.EqualTo:
                        return new ValidationResult{IsValid = resultNum == Limit};
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return new ValidationResult{IsValid = false};
        }
    }
}