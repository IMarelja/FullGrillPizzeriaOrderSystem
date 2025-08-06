using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelConstants
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalRangeAttribute : ValidationAttribute
    {
        private readonly int _precision;
        private readonly int _scale;

        public DecimalRangeAttribute(int precision, int scale)
        {
            _precision = precision;
            _scale = scale;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is decimal decimalValue)
            {
                var parts = decimalValue.ToString().Split('.');
                var integerDigits = parts[0].Length;
                var decimalDigits = parts.Length > 1 ? parts[1].Length : 0;

                if (integerDigits + decimalDigits > _precision || decimalDigits > _scale)
                {
                    return new ValidationResult($"Value exceeds allowed precision ({_precision}) or scale ({_scale}).");
                }
            }
            return ValidationResult.Success;
        }
    }

}
