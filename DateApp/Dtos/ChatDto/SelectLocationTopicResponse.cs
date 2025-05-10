namespace DateApp.Dtos.ChatDto
{
    public class SelectLocationTopicResponse
    {
        public string GroupName { get; set; } = string.Empty;
        public List<ChatMessageDto> RecentMessages { get; set; } = [];
    }
}