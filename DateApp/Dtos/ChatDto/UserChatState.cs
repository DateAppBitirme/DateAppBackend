
namespace DateApp.Dtos.ChatDto
{
    public class UserChatState
    {
        public string ConnectionId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? CurrentGroupName { get; set; }  // null ise hiçbir grupta değil  
    }
}