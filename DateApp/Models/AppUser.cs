using DateApp.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace DateApp.Models
{
    public class AppUser : IdentityUser
    {

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }
        
        public bool IsOnline { get; set; }
        
        public DateTime LastSeen { get; set; }
    }
}
