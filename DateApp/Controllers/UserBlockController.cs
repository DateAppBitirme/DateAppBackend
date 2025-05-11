using System.Security.Claims;
using DateApp.Interfaces;
using DateApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DateApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserBlockController : ControllerBase
    {
        private readonly IUserBlockService _blockService;
        private readonly UserManager<AppUser> _userManager;

        public UserBlockController(
            IUserBlockService blockService,
            UserManager<AppUser> userManager)
        {
            _blockService = blockService;
            _userManager = userManager;
        }
        [HttpPost("{blockedId}")]
        public async Task<IActionResult> Block(string blockedId)
        {
            var blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (blockerId == blockedId) return BadRequest("Kendini engelleyemezsin.");

            var blockedUser = await _userManager.FindByIdAsync(blockedId);
            if (blockedUser == null) return NotFound("Kullanıcı bulunamadı.");

            var ok = await _blockService.BlockUserAsync(blockerId!, blockedId);
            if (!ok) return BadRequest("Zaten engelledin veya işlem başarısız.");

            return NoContent();
        }

        [HttpDelete("{blockedId}")]
        public async Task<IActionResult> Unblock(string blockedId)
        {
            var blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ok = await _blockService.UnblockUserAsync(blockerId!, blockedId);
            if (!ok) return BadRequest("Engel bulunamadı.");
            return NoContent();
        }
    }
}