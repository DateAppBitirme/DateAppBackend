using DateApp.Dtos.AccountDto;
using DateApp.Interfaces;
using DateApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
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
                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "user");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto { 
                                Username = appUser.UserName, 
                                Email = appUser.Email,
                                Token = await _tokenService.CreateToken(appUser)
                            }

                            );
                    }
                    else
                    {
                        return BadRequest(new 
                        { message = "Rol atanamadı.", 
                          errors = roleResult.Errors.Select(e => e.Description) 
                        });

                    }
                }
                else
                {
                    // return BadRequest(new { message = "Kullanıcı oluşturulamadı." });
                    return BadRequest(new
                    {
                        message = "Kullanıcı oluşturulamadı.",
                        errors = createdUser.Errors.Select(e => e.Description)
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
                var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
                if (appUser == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı." });
                }
                var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    return Unauthorized("Kullanıcı adı veya parola hatalı!");
                }

                return Ok(new NewUserDto
                {
                    Username = appUser.UserName,
                    Email = appUser.Email,
                    Token = await _tokenService.CreateToken(appUser)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
