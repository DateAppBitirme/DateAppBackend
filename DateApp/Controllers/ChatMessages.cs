using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DateApp.Core.utils;
using DateApp.Data;
using DateApp.Dtos.ChatDto;
using DateApp.Interfaces;
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
    public class ChatMessages : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActiveUserTracker _userTracker; // Aktif kullanıcıları takip için
        

        public ChatMessages(ApplicationDbContext context, UserManager<AppUser> userManager, IActiveUserTracker userTracker)
        {
            _context = context;
            _userManager = userManager;
            _userTracker = userTracker;
        }
        [HttpPost("select-location-topic")]
        public async Task<IActionResult> SelectLocationTopic([FromBody] SelectLocationTopicRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Konu geçerli mi ?
            var validTopics = new List<string> { "Spor", "Sanat", "Kahve", "Sinema" }; // Örnek
            if (!validTopics.Contains(request.Topic, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { Message = "Geçersiz sohbet konusu." });
            }

            var gridCellId = LocationUtils.GetGridCellId(request.Latitude, request.Longitude);
            var groupName = $"{request.Topic.ToUpperInvariant()}_{gridCellId}";

            // O gruba ait son N mesajı çek
            var recentMessages = await _context.ChatMessages
                .Where(m => m.GroupName == groupName
                        && !_context.UserBlocks
                        .Any(ub => ub.BlockerId == userId
                        && ub.BlockedId == m.SenderUserId))
                .OrderByDescending(m => m.Timestamp)
                .Take(40) // Son 40 mesaj
                .Include(m => m.SenderUser) // Gönderen kullanıcı bilgisini çekmek için
                .Select(m => new ChatMessageDto
                {
                    SenderUserId = m.SenderUserId,
                    SenderUserName = m.SenderUser.UserName!,
                    MessageContent = m.MessageContent,
                    Timestamp = m.Timestamp
                })
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(new SelectLocationTopicResponse
            {
                GroupName = groupName,
                RecentMessages = recentMessages
            });
        }

    }
}