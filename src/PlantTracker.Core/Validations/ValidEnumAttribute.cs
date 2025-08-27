using System.ComponentModel.DataAnnotations;

namespace PlantTracker.Core.Validations
{
    public class ValidEnumAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var enumType = value.GetType();
            if (!enumType.IsEnum)
            {
                return ValidationResult.Success;
            }

            return !Enum.IsDefined(enumType, value)
                ? new ValidationResult(ErrorMessage ?? $"Invalid value for {enumType.Name}")
                : ValidationResult.Success;
        }
    }
}