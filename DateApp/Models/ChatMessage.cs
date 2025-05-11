
namespace DateApp.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; } = null!;
        public string SenderUserId { get; set; } = null!;
        public virtual AppUser SenderUser { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}