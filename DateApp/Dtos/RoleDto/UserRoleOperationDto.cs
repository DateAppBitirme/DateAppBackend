using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.RoleDto
{
    public class UserRoleOperationDto
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Rol adı zorunludur.")]
        public string RoleName { get; set; }
    }
}
