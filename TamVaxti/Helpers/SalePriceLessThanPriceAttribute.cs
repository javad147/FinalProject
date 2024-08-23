using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TamVaxti.Helpers
{
    public class SalePriceLessThanPriceAttribute : ValidationAttribute
    {
        private readonly string _pricePropertyName;
        private readonly string _salePricePropertyName;

        public SalePriceLessThanPriceAttribute(string pricePropertyName, string salePricePropertyName)
        {
            _pricePropertyName = pricePropertyName;
            _salePricePropertyName = salePricePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the 'Price' property value
            var priceProperty = validationContext.ObjectType.GetProperty(_pricePropertyName);
            var salePriceProperty = validationContext.ObjectType.GetProperty(_salePricePropertyName);

            if (priceProperty == null)
                return new ValidationResult($"Unknown property: {_pricePropertyName}");

            if (salePriceProperty == null)
                return new ValidationResult($"Unknown property: {_salePricePropertyName}");

            // Get the values of both properties
            var priceValue = (decimal)priceProperty.GetValue(validationContext.ObjectInstance);
            var salePriceValue = (decimal)salePriceProperty.GetValue(validationContext.ObjectInstance);

            // Compare the values
            if (salePriceValue >= priceValue)
            {
                return new ValidationResult($"Sale price must be less than the regular price.");
            }

            return ValidationResult.Success;
        }
    }
}

