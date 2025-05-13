namespace DateApp.Dtos.ComplaintRequestDto
{
    public class ComplaintAndRequestDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? UserName { get; set; }
    }
}
