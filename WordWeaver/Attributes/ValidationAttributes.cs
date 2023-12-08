#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).

using System.ComponentModel.DataAnnotations;

namespace WordWeaver.Validations;

public class MinimumAgeAttribute(int minimumAge) : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {

        ErrorMessage = $"You must be at least {minimumAge} years old.";

        if (value is DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;

            if (birthday.Date > today.AddYears(-age))
            {
                age--;
            }

            // Check if the age is at least the specified minimum age
            if (age < minimumAge)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}