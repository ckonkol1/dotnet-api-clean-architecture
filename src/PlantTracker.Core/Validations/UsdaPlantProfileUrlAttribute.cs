using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PlantTracker.Core.Validations;

public class UsdaPlantProfileUrlAttribute : ValidationAttribute
{
    private const string RequiredPrefix = "https://plants.usda.gov/plant-profile";

    private static readonly Regex SqlInjectionPattern = new Regex(
        @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|SCRIPT|JAVASCRIPT|VBSCRIPT)\b)|"
        + @"(--|/\*|\*/|;|'|""|<|>|&|%|@|\+|\||\\|\^|\$|\#|\!|\?|\*|\(|\)|\[|\]|\{|\})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var url = value.ToString();
        if (url != null && !url.StartsWith(RequiredPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult($"Url does not start with the required prefix: {RequiredPrefix}");
        }

        if (url != null && SqlInjectionPattern.IsMatch(url))
        {
            return new ValidationResult($"Invalid Url");
        }

        return ValidationResult.Success;
    }
}