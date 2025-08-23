using System.ComponentModel.DataAnnotations;

namespace PlantTrackerCleanArchitectureApi.Core.Validations
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

            if (!Enum.IsDefined(enumType, value))
            {
                return new ValidationResult(ErrorMessage ?? $"Invalid value for {enumType.Name}");
            }

            return ValidationResult.Success;
        }
    }
}