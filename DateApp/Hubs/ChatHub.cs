using DateApp.Data;
using DateApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace DateApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;

        public ChatHub(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier!;

            // Alıcı kontrolü
            var receiver = await _dbContext.Users.FindAsync(receiverId);
            if (receiver == null) throw new HubException("Kullanıcı bulunamadı");

            // Mesajı kaydet
            var newMessage = new PrivateMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message.Trim(),
                SentAt = DateTime.UtcNow
            };

            _dbContext.PrivateMessages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            // Gerçek zamanlı gönderim
            await Clients.Users(receiverId, senderId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = newMessage.Content,
                SentAt = newMessage.SentAt
            });
        }

        // Çevrimiçi durum güncelleme
        public override async Task OnConnectedAsync()
        {
            await UpdateOnlineStatus(true);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await UpdateOnlineStatus(false);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task UpdateOnlineStatus(bool isOnline)
        {
            var user = await _dbContext.Users.FindAsync(Context.UserIdentifier);
            if (user != null)
            {
                user.IsOnline = isOnline;
                user.LastSeen = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
