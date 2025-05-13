using System.ComponentModel.DataAnnotations;

namespace DateApp.Dtos.ComplaintRequestDto
{
    public class CreateRequestDto
    {
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public int? RequestTypeId { get; set; }
    }
}
