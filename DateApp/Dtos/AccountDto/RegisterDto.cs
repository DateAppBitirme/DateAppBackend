using System.ComponentModel.DataAnnotations;
using DateApp.Core.Enums;
using static DateApp.Models.AppUser;

namespace DateApp.Dtos.AccountDto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required!")]
        [StringLength(100, MinimumLength = 3)]
        public string? Username { get; init; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; init; }

        [Required(ErrorMessage = "Password is required!")]
        public string? Password { get; init; }

        [Required]
        [RegularExpression(@"^\+905\d{9}$",
            ErrorMessage = "Please enter a valid phone number. Example: +905xxxxxxxxx")]
        public string? PhoneNumber { get; init; }

        [Required(ErrorMessage = "Gender is required!")]
        public Gender? Gender { get; init; }

        [Required(ErrorMessage = "Date of birth is required!")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DateValidation), "ValidateAge")]
        public DateTime? DateOfBirth { get; init; }

    }

    public static class DateValidation
    {
        public static ValidationResult? ValidateAge(DateTime? dateOfBirth, ValidationContext context)
        {
            if (!dateOfBirth.HasValue)
                return ValidationResult.Success; // for update - profile

            var age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value > DateTime.Now.AddYears(-age)) age--;

            return age >= 18
                ? ValidationResult.Success
                : new ValidationResult("You must be at least 18 years old to register!");
        }
    }
}
