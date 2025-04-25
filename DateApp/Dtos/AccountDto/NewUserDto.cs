using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.AccountDto
{
    public class NewUserDto
    {   
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public bool? EmailConfirmed { get; set; }
    }
}
