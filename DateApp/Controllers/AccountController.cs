using DateApp.Dtos.AccountDto;
using DateApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    return Ok(
                            new NewUserDto
                            {
                                Username = user.UserName,
                                Email = user.Email,
                            }); 
                }
                else
                {
                    // return BadRequest(new { message = "Kullanıcı oluşturulamadı." });
                    return BadRequest(new
                    {
                        message = "Kullanıcı oluşturulamadı.",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı." });
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    return Unauthorized("Kullanıcı adı veya parola hatalı!");
                }

                return Ok(new NewUserDto
                {
                    Username = user.UserName,
                    Email = user.Email,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
