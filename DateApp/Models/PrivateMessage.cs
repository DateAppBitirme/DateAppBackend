using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DateApp.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }

        [Required]
        public string? SenderId { get; set; }

        [Required]
        public string? ReceiverId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string? Content { get; set; }

        public DateTime SentAt { get; set; }

        [ForeignKey("SenderId")]
        public AppUser? Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public AppUser? Receiver { get; set; }
    }
}
