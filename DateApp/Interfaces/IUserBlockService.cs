namespace DateApp.Interfaces
{
    public interface IUserBlockService
    {
        Task<bool> BlockUserAsync(string blockerId, string blockedId);
        Task<bool> UnblockUserAsync(string blockerId, string blockedId);
        Task<bool> IsBlockedAsync(string blockerId, string otherUserId);
    }
}