using DateApp.Dtos.ChatDto;

namespace DateApp.Interfaces
{
    public interface IActiveUserTracker
    {
        void UserConnected(string userId, string connectionId);
        void UserDisconnected(string connectionId);

        void UserJoinedGroup(string connectionId, string groupName, string userId);
        void UserLeftGroup(string connectionId, string groupName);

        UserChatState? GetByConnection(string connectionId);
        IEnumerable<UserChatState> GetUsersInGroup(string groupName);
    }
}