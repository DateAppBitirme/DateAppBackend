
namespace DateApp.Dtos.ChatDto
{
    public class ChatMessageDto
    {
        public string SenderUserId { get; set; } = string.Empty;
        public string SenderUserName { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}