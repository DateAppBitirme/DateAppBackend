using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.AccountDto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email adresi mevcut değil")]
        public string? Email { get; set; }

    }
}
