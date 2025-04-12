using DateApp.Dtos.AccountDto;
using DateApp.Dtos.RoleDto;
using DateApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsersWithRole()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userRoles = new List<object>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userRoles.Add(new
                    {
                        user.Id,
                        user.UserName,
                        Roles = roles
                    });
                }
                return Ok(userRoles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto deleteUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _userManager.FindByNameAsync(deleteUserDto.Username);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new {Message = $"{deleteUserDto.Username} kullanıcısı başarıyla silindi. "});
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleManager.Roles.Select(r => new {r.Name}).ToListAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var roleExists = await _roleManager.RoleExistsAsync(roleDto.RoleName);
                if (roleExists)
                {
                    return BadRequest("Bu rol zaten mevcut.");
                }
                var role = new IdentityRole(roleDto.RoleName);
                var result = await _roleManager.CreateAsync(role);
                
                if (result.Succeeded)
                {
                    return Ok("Rol başarıyla oluşturuldu.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] RoleDto roleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var role = await _roleManager.FindByNameAsync(roleDto.RoleName);
                if (role == null)
                {
                    return NotFound("Rol bulunamadı.");
                }
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Ok("Rol başarıyla silindi.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleOperationDto userRoleOperationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByNameAsync(userRoleOperationDto.Username);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }

                if (!await _roleManager.RoleExistsAsync(userRoleOperationDto.RoleName))
                {
                    return BadRequest("Geçersiz rol");
                }

                if (await _userManager.IsInRoleAsync(user, userRoleOperationDto.RoleName))
                {
                    return BadRequest("Kullanıcı zaten bu role sahip.");
                }

                var result = await _userManager.AddToRoleAsync(user, userRoleOperationDto.RoleName);
                if (result.Succeeded)
                {
                    return Ok("Rol başarıyla atandı.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("RemoveRoleFromUser")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleOperationDto userRoleOperationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _userManager.FindByNameAsync(userRoleOperationDto.Username);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }
                if (!await _userManager.IsInRoleAsync(user, userRoleOperationDto.RoleName))
                {
                    return BadRequest("Kullanıcı bu role sahip değil.");
                }

                var roleExists = await _roleManager.RoleExistsAsync(userRoleOperationDto.RoleName);
                if (!roleExists)
                {
                    return NotFound("Rol bulunamadı.");
                }
                var result = await _userManager.RemoveFromRoleAsync(user, userRoleOperationDto.RoleName);
                if (result.Succeeded)
                {
                    return Ok(new { Message = $"{userRoleOperationDto.Username} kullanıcısı {userRoleOperationDto.RoleName} rolünden çıkarıldı" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
