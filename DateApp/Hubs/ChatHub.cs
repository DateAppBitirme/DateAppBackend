using System;
using System.Threading.Tasks;
using DateApp.Data;
using DateApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DateApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ApplicationDbContext dbContext, ILogger<ChatHub> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Bağlantı açıldığında kullanıcıyı online yap
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await UpdateOnlineStatus(userId, true);
            }
            await base.OnConnectedAsync();
        }

        // Bağlantı kapandığında kullanıcıyı offline yap
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await UpdateOnlineStatus(userId, false);
            }
            await base.OnDisconnectedAsync(exception);
        }

        // Mesaj gönderme metodu
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(senderId))
            {
                throw new HubException("Sender ID not found.");
            }

            var receiver = await _dbContext.Users.FindAsync(receiverId);
            if (receiver == null)
            {
                throw new HubException("No recipient found.");
            }

            var newMessage = new PrivateMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message?.Trim(),
                SentAt = DateTime.UtcNow
            };

            _dbContext.PrivateMessages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            var payload = new
            {
                newMessage.Id,
                newMessage.SenderId,
                newMessage.ReceiverId,
                newMessage.Content,
                SentAt = newMessage.SentAt
            };

            // Sadece alıcıya gönder
            await Clients.User(receiverId).SendAsync("ReceiveMessage", payload);
            // Gönderene echo (isteğe bağlı)
            await Clients.Caller.SendAsync("ReceiveMessage", payload);
        }

        // Online durum güncelleme yardımcı metodu
        private async Task UpdateOnlineStatus(string userId, bool isOnline)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastSeen = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
