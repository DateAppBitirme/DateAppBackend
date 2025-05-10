using DateApp.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace DateApp.Models
{
    public class AppUser : IdentityUser
    {

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool? IsOnline { get; set; }

        public DateTime? LastSeen { get; set; }
        public string? GridCellId { get; set; }


        public ICollection<UserBlock> BlockedUsers { get; set; } = new List<UserBlock>();
        public ICollection<UserBlock> BlockedByUsers { get; set; } = new List<UserBlock>();
    }
}
