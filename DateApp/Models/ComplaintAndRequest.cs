using System.ComponentModel.DataAnnotations;
using static DateApp.Core.Enums.ComplaintRequest;

namespace DateApp.Models
{
    public class ComplaintAndRequest
    {
        public int Id { get; set; }
        [Required]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long.")]
        public string? Title { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long.")]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public string? UserId { get; set; }
        public int? ComplaintTypeId { get; set; }
        public int? RequestTypeId { get; set; }
        
        public virtual AppUser? User { get; set; }
        public virtual ComplaintType? ComplaintType { get; set; }
        public virtual RequestType? RequestType { get; set; }
    }
}
