using System.Net;
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

                if (!string.IsNullOrEmpty(registerDto.PhoneNumber))
                {
                    var phoneExists = await _userManager.Users
                                           .AnyAsync(u => u.PhoneNumber == registerDto.PhoneNumber);
                    if (phoneExists)
                    {
                        ModelState.AddModelError(nameof(registerDto.PhoneNumber), "This phone number is already taken.");
                        return BadRequest(ModelState);
                    }
                }

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    Gender = registerDto.Gender!.Value,
                    DateOfBirth = registerDto.DateOfBirth!.Value,
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password!);
                if (!createdUser.Succeeded)
                {
                    foreach (var error in createdUser.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                var roleResult = await _userManager.AddToRoleAsync(appUser, "user");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "The role could not be assigned.",
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
                await emailService.SendEmailAsync(appUser.Email!, "E-posta Doğrulama", emailBody);

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
                    return NotFound(new { message = "User could not found!" });
                }

                if (!await _userManager.IsEmailConfirmedAsync(appUser))
                {
                    return Unauthorized("Your email address is not verified!");
                }
                var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginDto.Password!, false);

                if (!result.Succeeded)
                {
                    return Unauthorized("Wrong username or password!");
                }

                return Ok(new NewUserDto
                {
                    Username = appUser.UserName!,
                    Email = appUser.Email!,
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto, [FromServices] IEmailService emailService)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(forgotPasswordDto.Email))
                {
                    return BadRequest(new { message = "Email is required." });
                }

                var appUser = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (appUser == null)
                {
                    return Ok(new { message = "Email sent!!!" });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                var urlEncodedToken = WebUtility.UrlEncode(token);
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var resetLink = $"{baseUrl}/reset-password.html?userId={appUser.Id}&token={urlEncodedToken}";

                var emailBody = $@"
                    <html>
                    <body>
                        <p>Merhaba,</p>
                        <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                        <p><a href='{resetLink}'>Şifre Sıfırla</a></p>
                        <p>Eğer siz şifre sıfırlama talebinde bulunmadıysanız, bu e-postayı dikkate almayınız.</p>
                        <p>Saygılarımızla.</p>
                    </body>
                    </html>
                ";

                /*..*/

                await emailService.SendEmailAsync(appUser.Email!, "Şifre Sıfırlama", emailBody);

                return Ok(new { message = "Email sent.", resetLink });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (resetPasswordDto.Password != resetPasswordDto.ConfirmPassword)
                {
                    return BadRequest(new { message = "Passwords are not matched!" });
                }

                if (string.IsNullOrEmpty(resetPasswordDto.UserId))
                {
                    return NotFound();
                }

                var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid request!" });
                }

                var result = await _userManager.ResetPasswordAsync(
                    user, resetPasswordDto.Token!, resetPasswordDto.Password!);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Password reset successfully." });
                }

                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Password reset failed!", errors });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Something is happened: {ex.Message}" });
            }

        }

    }
}
