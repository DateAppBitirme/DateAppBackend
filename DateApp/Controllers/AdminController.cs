using DateApp.Data;
using DateApp.Dtos.AccountDto;
using DateApp.Dtos.ComplaintRequestDto;
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
        private readonly ApplicationDbContext _context;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
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

                // İlişkili kayıtları sil
                var userMessages = await _context.PrivateMessages
                    .Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
                    .ToListAsync();
                _context.PrivateMessages.RemoveRange(userMessages);

                var userChatMessages = await _context.ChatMessages
                    .Where(m => m.SenderUserId == user.Id)
                    .ToListAsync();
                _context.ChatMessages.RemoveRange(userChatMessages);

                var userBlocks = await _context.UserBlocks
                    .Where(b => b.BlockerId == user.Id || b.BlockedId == user.Id)
                    .ToListAsync();
                _context.UserBlocks.RemoveRange(userBlocks);

                var userComplaints = await _context.ComplaintAndRequests
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync();
                _context.ComplaintAndRequests.RemoveRange(userComplaints);

                await _context.SaveChangesAsync();

                // Son olarak kullanıcıyı sil
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Message = $"{deleteUserDto.Username} kullanıcısı başarıyla silindi. " });
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
                var roles = await _roleManager.Roles.Select(r => new { r.Name }).ToListAsync();
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

        [HttpGet("GetAllComplaintAndRequests")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintsAndRequests = await _context.ComplaintAndRequests
                .Include(c => c.User)
                .Where(c => !c.IsDeleted)
                .Select(c => new ComplaintAndRequestDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive,
                    UserName = c.User != null ? c.User.UserName : ""
                })
                    .ToListAsync();

                return Ok(complaintsAndRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetAllComplaint")]
        public async Task<IActionResult> GetAllComplaint()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaints = await _context.ComplaintAndRequests
                    .Include(c => c.User)
                    .Where(c => c.IsActive && !c.IsDeleted && c.ComplaintTypeId != null)
                    .Select(c => new ComplaintAndRequestDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        IsActive = c.IsActive,
                        UserName = c.User != null ? c.User.UserName : ""
                    })
                    .ToListAsync();
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllRequest")]
        public async Task<IActionResult> GetAllRequest()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var requests = await _context.ComplaintAndRequests
                .Include(c => c.User)
                .Where(c => c.IsActive && !c.IsDeleted && c.RequestTypeId != null)
                .Select(c => new ComplaintAndRequestDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive,
                    UserName = c.User != null ? c.User.UserName : ""
                })
                    .ToListAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteComplaintAndRequest/{id}")]
        public async Task<IActionResult> DeleteComplaintAndRequest(int id) //Soft delete yaptım. Yani veriler silinse de görünebilecek.
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintAndRequest = await _context.ComplaintAndRequests.FindAsync(id);
                if (complaintAndRequest == null)
                {
                    return NotFound("Şikayet veya istek bulunamadı.");
                }
                complaintAndRequest.IsDeleted = true;
                _context.ComplaintAndRequests.Update(complaintAndRequest);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Şikayet veya istek başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("ResolveComplaintAndRequest/{id}")]
        public async Task<IActionResult> ResolveComplaintAndRequest(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintAndRequest = await _context.ComplaintAndRequests.FindAsync(id);
                if (complaintAndRequest == null)
                {
                    return NotFound("Şikayet veya istek bulunamadı.");
                }
                complaintAndRequest.IsActive = false;
                _context.ComplaintAndRequests.Update(complaintAndRequest);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Şikayet veya istek başarıyla çözüldü." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



    }
}
