using System.Collections.Concurrent;
using DateApp.Dtos.ChatDto;
using DateApp.Interfaces;

namespace DateApp.Services
{
    public class InMemoryActiveUserTracker : IActiveUserTracker
    {
        // Key: ConnectionId, Value: UserChatState
        private readonly ConcurrentDictionary<string, UserChatState> _states
            = new ConcurrentDictionary<string, UserChatState>();

        public void UserConnected(string userId, string connectionId)
        {
            var state = new UserChatState { UserId = userId, ConnectionId = connectionId };
            _states[connectionId] = state;
        }

        public void UserDisconnected(string connectionId)
        {
            //'out _' => bu değeri alırım ama hiç kullanmayacağım, isme ihtiyacım yok
            _states.TryRemove(connectionId, out _);
        }

        public void UserJoinedGroup(string connectionId, string groupName, string userId)
        {
            if (_states.TryGetValue(connectionId, out var userState))
            {
                userState.CurrentGroupName = groupName;
                userState.UserId = userId; // Ensure UserId is set
            }
            else // Should ideally be connected first
            {
                _states.TryAdd(connectionId, new UserChatState { UserId = userId, ConnectionId = connectionId, CurrentGroupName = groupName });
            }
        }

        public void UserLeftGroup(string connectionId, string groupName)
        {
            if (_states.TryGetValue(connectionId, out var state)
            && state.CurrentGroupName == groupName)
            {
                state.CurrentGroupName = null;
            }
        }
        public UserChatState? GetByConnection(string connectionId)
        => _states.TryGetValue(connectionId, out var s) ? s : null;

        public IEnumerable<UserChatState> GetUsersInGroup(string groupName)
            => _states.Values.Where(s => s.CurrentGroupName == groupName);
    }
}