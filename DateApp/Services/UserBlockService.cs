
using DateApp.Data;
using DateApp.Interfaces;
using DateApp.Models;

namespace DateApp.Services
{
    public class UserBlockService : IUserBlockService
    {
        private readonly ApplicationDbContext _context;

        public UserBlockService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BlockUserAsync(string blockerId, string blockedId)
        {
            if (blockerId == blockedId) return false;

            var exists = await _context.UserBlocks.FindAsync(blockerId, blockedId);
            if (exists != null) return false;

            _context.UserBlocks.Add(new UserBlock
            {
                BlockerId = blockerId,
                BlockedId = blockedId,
                BlockedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(string blockerId, string blockedId)
        {
            var entity = await _context.UserBlocks.FindAsync(blockerId, blockedId);
            if (entity == null) return false;
            _context.UserBlocks.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsBlockedAsync(string blockerId, string otherUserId)
        {
            var entity = await _context.UserBlocks.FindAsync(blockerId, otherUserId);
            return entity != null;
        }
    }
}