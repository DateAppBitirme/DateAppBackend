using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.RoleDto
{
    public class RoleDto
    {
        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Rol adı 3-15 karakter arası olmalıdır.")]
        public string RoleName { get; set; }
    }
}
