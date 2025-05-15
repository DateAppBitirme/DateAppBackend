using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using DateApp.Data;
using Microsoft.EntityFrameworkCore;
using DateApp.Dtos.ChatDto;

namespace DateApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ChatController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(string userId, [FromQuery] int page = 1)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var messages = await _dbContext.PrivateMessages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                           (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * 20)
                .Include(m => m.Receiver)
                .Take(40)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.Receiver!.UserName,
                    m.ReceiverId,
                    m.Content,
                    m.SentAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("recent-conversations")]
        public async Task<IActionResult> GetRecentConversations() 
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            try
            {
                var conversationPartners = await _dbContext.PrivateMessages
                    .Where(pm => pm.SenderId == currentUserId || pm.ReceiverId == currentUserId)
                    .GroupBy(pm => pm.SenderId == currentUserId ? pm.ReceiverId : pm.SenderId) 
                    .Select(g => new
                    {
                        OtherUserId = g.Key, 
                        LastMessageTimestamp = g.Max(m => m.SentAt) 
                    })
                    .OrderByDescending(c => c.LastMessageTimestamp)                                                    
                    .ToListAsync();

              

                if (!conversationPartners.Any())
                {
                    return Ok(new List<RecentConversationDto>());
                }

                var otherUserIds = conversationPartners
                                    .Select(c => c.OtherUserId)
                                    .Where(id => id != null) 
                                    .Distinct()
                                    .ToList();

                if (!otherUserIds.Any())
                { 
                    return Ok(new List<RecentConversationDto>());
                }
                
                var otherUsersMap = await _dbContext.Users
                                                 .Where(u => otherUserIds.Contains(u.Id))
                                                 .ToDictionaryAsync(u => u.Id, u => u.UserName); 
 
                var result = conversationPartners.Select(c =>
                {
                    if (c.OtherUserId == null) return null; // Bu durum olmamalı
                    return new RecentConversationDto
                    {
                        OtherUserId = c.OtherUserId,
                        OtherUserName = otherUsersMap.TryGetValue(c.OtherUserId, out var userName) ? (userName ?? "Kullanıcı Adı Yok") : "Bilinmeyen Kullanıcı"    
                    };
                })
                .Where(dto => dto != null) 
                .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Son konuşmalar alınırken sunucuda bir hata oluştu.");
            }
        }
    }
}
