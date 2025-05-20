using DateApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {

        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<ComplaintAndRequest> ComplaintAndRequests { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole { Id = "1",Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2",Name = "user", NormalizedName = "USER" }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            builder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(u => u.PhoneNumber).IsUnique();
            });

            builder.Entity<PrivateMessage>()
                .HasOne(pm => pm.Sender)
                .WithMany()
                .HasForeignKey(pm => pm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PrivateMessage>()
                .HasOne(pm => pm.Receiver)
                .WithMany()
                .HasForeignKey(pm => pm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);


            // ChatMessage için yapılandırmalar
            builder.Entity<ChatMessage>()
                .HasOne(m => m.SenderUser)
                .WithMany() // Kullanıcının birçok mesajı olabilir
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silinirse mesajları ne olacak?

            builder.Entity<UserBlock>()
            .HasKey(ub => new { ub.BlockerId, ub.BlockedId })
            .IsClustered(false);

            builder.Entity<UserBlock>()
                .HasOne(ub => ub.Blocker)
                .WithMany(u => u.BlockedUsers)      // AppUser’da ICollection<UserBlock> BlockedUsers
                .HasForeignKey(ub => ub.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserBlock>()
                .HasOne(ub => ub.Blocked)
                .WithMany(u => u.BlockedByUsers)    // AppUser’da ICollection<UserBlock> BlockedByUsers
                .HasForeignKey(ub => ub.BlockedId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kullanıcı ile ilişki
            builder.Entity<ComplaintAndRequest>()
                 .HasOne(cr => cr.User)
                 .WithMany()
                 .HasForeignKey(cr => cr.UserId)
                 .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
