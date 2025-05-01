using System.ComponentModel.DataAnnotations;

namespace DateApp.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(400)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public double Latitude { get; set; } 

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string? OrganizerId { get; set; } 

        public bool IsPublic { get; set; } 
    }
}
