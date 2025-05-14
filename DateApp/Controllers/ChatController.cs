using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using DateApp.Data;
using Microsoft.EntityFrameworkCore;

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

        // [HttpGet("search")]
        // public async Task<IActionResult> SearchUsers([FromQuery] string query)
        // {
        //     var users = await _dbContext.Users
        //         .Where(u => u.UserName.Contains(query) || u.Email.Contains(query))
        //         .Select(u => new
        //         {
        //             u.Id,
        //             u.UserName,
        //             u.Email,
        //             u.IsOnline,
        //             u.LastSeen
        //         })
        //         .ToListAsync();

        //     return Ok(users);
        // }
    }
}
