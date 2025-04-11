using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.AccountDto
{
    public class DeleteUserDto
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; }  
    }
}
