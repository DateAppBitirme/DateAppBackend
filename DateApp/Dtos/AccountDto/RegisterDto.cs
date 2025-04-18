using System.ComponentModel.DataAnnotations;
using static DateApp.Models.AppUser;

namespace DateApp.Dtos.AccountDto
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", 
            ErrorMessage = "Telefon numaranız geçerli bir uluslararası formatta olmalıdır.")]
        public string PhoneNumber { get; set; }

        [Required]
        public gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DateValidation), "ValidateAge")]
        public DateTime? DateOfBirth { get; set; }

        [MinLength(1, ErrorMessage = "En az bir ilgi alanı eklenmelidir.")]
        public List<string> Interests { get; set; } = new List<string>();
    }

    public static class DateValidation
    {
        public static ValidationResult ValidateAge(DateTime? dateOfBirth, ValidationContext context)
        {
            if (!dateOfBirth.HasValue)
                return new ValidationResult("Doğum tarihi zorunludur.");

            var age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value > DateTime.Now.AddYears(-age)) age--;

            return age >= 18 
                ? ValidationResult.Success 
                : new ValidationResult("En az 18 yaşında olmanız gerekmektedir.");
        }
    }
}
