using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.AccountDto
{
    public record LoginDto
    {
        [Required(ErrorMessage = "Username is required!")]
        public string? Username { get; init; }

        [Required(ErrorMessage = "Password is required!")]
        public string? Password { get; init; }
    }
}
