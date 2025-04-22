using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DateApp.Dtos.AccountDto
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "UserId is required!")]
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Token is required!")]
        public string? Token { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords are not matched!")]
        public string? ConfirmPassword { get; set; }
    }
}