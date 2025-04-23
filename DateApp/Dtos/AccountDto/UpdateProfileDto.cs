using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DateApp.Core.Enums;

namespace DateApp.Dtos.AccountDto
{
    public class UpdateProfileDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [RegularExpression(@"^\+905\d{9}$",
            ErrorMessage = "Please enter a valid phone number. Example: +905xxxxxxxxx")]
        public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(DateValidation), "ValidateAge")]
        public DateTime? DateOfBirth { get; set; }
    }


}