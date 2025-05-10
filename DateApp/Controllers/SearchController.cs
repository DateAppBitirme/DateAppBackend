using System.Security.Claims;
using DateApp.Data;
using DateApp.Dtos.AccountDto;
using DateApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public SearchController(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: api/Users/search?searchTerm=ahmet&pageNumber=1&pageSize=10
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            {
                return Ok(new List<UserSearchResultDto>());
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var normalizedSearchTerm = searchTerm.ToUpperInvariant();

            try
            {
                var userRole = await _roleManager.FindByNameAsync("User");
                if (userRole == null)
                {
                    return Ok(new List<UserSearchResultDto>());
                }

                var query = _context.Users
                    .Where(u => u.Id != currentUserId)
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == userRole.Id))
                    .Where(u =>
                        u.NormalizedUserName != null && u.NormalizedUserName.Contains(normalizedSearchTerm)
                    )
                    .Select(u => new UserSearchResultDto
                    {
                        UserId = u.Id,
                        UserName = u.UserName!,
                    });

                // Sayfalama uygula
                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500, "Arama sırasında bir sorun oluştu. Lütfen daha sonra tekrar deneyin.");
            }
        }
    }
}