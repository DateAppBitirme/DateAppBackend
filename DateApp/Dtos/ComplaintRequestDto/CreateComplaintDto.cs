using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.ComplaintRequestDto
{
    public class CreateComplaintDto
    {
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public int? ComplaintTypeId { get; set; }

    }
}
