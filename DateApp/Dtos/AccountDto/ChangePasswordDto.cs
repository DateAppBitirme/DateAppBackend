using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DateApp.Dtos.AccountDto
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required!")]
        public string? CurrentPassword { get; init;}

        [Required(ErrorMessage = "New password is required!")]
        public string? NewPassword { get; init; }

        [Required(ErrorMessage = "Confirm new password is required!")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match!")]
        public string? ConfirmNewPassword { get; init; }
    }
}