using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.AccountDto
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Kullanıcı ID gereklidir.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Token gereklidir.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır.", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Parolalar uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}
