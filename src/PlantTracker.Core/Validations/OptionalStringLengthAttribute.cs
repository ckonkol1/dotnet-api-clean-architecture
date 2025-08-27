namespace PlantTracker.Core.Validations;

using System.ComponentModel.DataAnnotations;

public class OptionalStringLengthAttribute(int minimumLength, int maximumLength) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string stringValue
            || stringValue == string.Empty
            || stringValue.Length >= minimumLength && stringValue.Length <= maximumLength)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            $"The field {validationContext.DisplayName} must have a length between {minimumLength} and {maximumLength} characters."
        );
    }
}