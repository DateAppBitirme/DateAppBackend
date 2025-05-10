using System.ComponentModel.DataAnnotations;


namespace DateApp.Dtos.ChatDto
{
    public class SelectLocationTopicRequest
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public string Topic { get; set; } = string.Empty;
    }
}