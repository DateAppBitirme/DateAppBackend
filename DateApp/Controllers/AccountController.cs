using DateApp.Dtos.AccountDto;
using DateApp.Interfaces;
using DateApp.Models;
using DateApp.Services;
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
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, [FromServices] IEmailService emailService)
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
                    PhoneNumber = registerDto.PhoneNumber,
                    Gender = registerDto.Gender,
                    DateOfBirth = registerDto.DateOfBirth,
                    Interests = registerDto.Interests
                };
 
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (!createdUser.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Kullanıcı oluşturulamadı.",
                        errors = createdUser.Errors.Select(e => e.Description)
                    });
                }
  
                var roleResult = await _userManager.AddToRoleAsync(appUser, "user");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Rol atanamadı.",
                        errors = roleResult.Errors.Select(e => e.Description)
                    });
                }

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = appUser.Id, token = emailConfirmationToken },
                    Request.Scheme
                );

                var emailBody = $"<p>Lütfen e-posta adresinizi doğrulamak için aşağıdaki bağlantıya tıklayın:</p><a href='{confirmationLink}'>E-posta Doğrula</a>";
                await emailService.SendEmailAsync(appUser.Email, "E-posta Doğrulama", emailBody);

                return Ok(new
                {
                    message = "Kayıt başarılı! Lütfen e-posta adresinizi doğrulamak için e-postanızı kontrol edin.",
                    user = new NewUserDto
                    {
                        Username = appUser.UserName,
                        Email = appUser.Email,
                        Token = await _tokenService.CreateToken(appUser)
                    }
                });
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
                if (!await _userManager.IsEmailConfirmedAsync(appUser))
                {
                    return Unauthorized("E-posta adresiniz doğrulanmamış.");
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
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok(new { message = "Çıkış yapıldı." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { message = "Geçersiz istek." });
                }
                var appUser = await _userManager.FindByIdAsync(userId);
                if (appUser == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı." });
                }
                var result = await _userManager.ConfirmEmailAsync(appUser, token);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "E-posta doğrulaması başarısız." });
                }
                return Ok(new { message = "E-posta adresiniz başarıyla doğrulandı!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
