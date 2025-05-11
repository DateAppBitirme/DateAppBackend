using System.Security.Claims;
using DateApp.Data;
using DateApp.Interfaces;
using DateApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace DateApp.Hubs
{
    [Authorize]
    public class LocationChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IActiveUserTracker _tracker;
        private readonly UserManager<AppUser> _userManager;
        IUserBlockService _blockService;
        private readonly ILogger<LocationChatHub> _logger;
        public LocationChatHub(ApplicationDbContext context, IActiveUserTracker userTracker, UserManager<AppUser> userManager, IUserBlockService blockService, ILogger<LocationChatHub> logger)
        {
            _context = context;
            _tracker = userTracker;
            _userManager = userManager;
            _blockService = blockService;
            _logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // Yetkisiz veya token'da userId olmayan bir bağlantı denemesi.
                Context.Abort(); // Bağlantıyı sonlandırır.
                return;
            }
            _tracker.UserConnected(userId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userState = _tracker.GetByConnection(Context.ConnectionId);
            if (userState != null && !string.IsNullOrEmpty(userState.CurrentGroupName))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userState.CurrentGroupName);
            }
            // Tracker’dan sil
            _tracker.UserDisconnected(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChatGroup(string groupName)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(groupName))
            {
                return;
            }

            // Önceki gruptan ayrıl 
            var userState = _tracker.GetByConnection(Context.ConnectionId);
            if (userState != null && !string.IsNullOrEmpty(userState.CurrentGroupName) && userState.CurrentGroupName != groupName)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userState.CurrentGroupName);
                _tracker.UserLeftGroup(Context.ConnectionId, userState.CurrentGroupName);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _tracker.UserJoinedGroup(Context.ConnectionId, groupName, userId);

            await Clients.Caller.SendAsync("JoinedGroupSuccess", groupName);
            _logger.LogInformation("Kullanıcı {UserId} ({ConnectionId}) {GroupName} grubuna katıldı ve JoinedGroupSuccess olayı gönderildi.", userId, Context.ConnectionId, groupName);

        }
        public async Task LeaveChatGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _tracker.UserLeftGroup(Context.ConnectionId, groupName);
        }
        public async Task SendMessageToGroup(string groupName, string message)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (string.IsNullOrEmpty(userId) || user == null)
            {
                await Clients.Caller.SendAsync("Error", "Mesaj gönderilemedi. Kullanıcı bulunamadı.");
                return;
            }

            // Gerçekten o grupta mı?
            var state = _tracker.GetByConnection(Context.ConnectionId);
            if (state?.CurrentGroupName != groupName)
            {
                await Clients.Caller.SendAsync("Error", "Bu gruba ait değilsiniz.");
                return;
            }

            var chatMessage = new ChatMessage
            {
                Id = Guid.NewGuid(),
                GroupName = groupName,
                SenderUserId = userId,
                MessageContent = message,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Gruba mesajı gönder
            var members = _tracker.GetUsersInGroup(groupName);

            await Clients.Group(groupName).SendAsync(
        "ReceiveMessage",
        chatMessage.SenderUserId, // Gönderenin ID'si
        user.UserName,            // Gönderenin kullanıcı adı
        message,
        chatMessage.Timestamp.ToString("o"));
        }
    }
}