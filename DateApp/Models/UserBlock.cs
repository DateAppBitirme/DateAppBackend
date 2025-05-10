namespace DateApp.Models
{
    public class UserBlock
    {
        public string BlockerId { get; set; } = string.Empty;    // Engelleyen kullanıcı
        public virtual AppUser Blocker { get; set; } = null!;

        public string BlockedId { get; set; } = string.Empty;    // Engellenen kullanıcı
        public virtual AppUser Blocked { get; set; } = null!;

        public DateTime BlockedAt { get; set; }  // Ne zaman engelledi
    }
}