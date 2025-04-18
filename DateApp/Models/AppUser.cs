using Microsoft.AspNetCore.Identity;

namespace DateApp.Models
{
    public class AppUser : IdentityUser
    {
        public enum gender
        {
            Male,//0
            Female,//1
            Other//2
        }

        public gender Gender { get; set; }

        public List<string> Interests { get; set; } = new List<string>();

        public DateTime? DateOfBirth { get; set; }

    }
}
