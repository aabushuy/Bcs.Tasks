using Web.Entity;
using Web.Storage;

namespace Web.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly List<DialogUser> _users;
        private readonly List<UserMessage> _userMessages;
        private readonly ILogger<IUserRepository> _logger;

        public UserRepository(IEntityStorage entityStorage, ILogger<IUserRepository> logger)
        {
            _users = entityStorage.Users;
            _userMessages = entityStorage.UserMessages;
            _logger = logger;
        }

        public Task<DialogUser?> GetUser(int id) => Task.FromResult(_users.FirstOrDefault(x => x.Id == id));

        public Task<DialogUser> GetOrAddUser(string name)
        {
            DialogUser? user = _users.FirstOrDefault(x => x.Name == name);

            if (user == null)
            {
                int userId = _users.Count == 0
                    ? 1
                    :_users.Max(u => u.Id) + 1;

                user = new DialogUser(userId, name);

                _users.Add(user);

                _logger.LogInformation($"Create a new user with id={userId}");
            }
            
            return Task.FromResult(user!);
        }

        public Task DeleteUser(int id)
        {
            DialogUser? user = _users.FirstOrDefault(x => x.Id == id) 
                ?? throw new KeyNotFoundException("No such user");

            _users.Remove(user);

            _logger.LogInformation($"Delete user with id={user.Id}");

            //TODO:if we use a real DB, it should be cascade on delete            
            UserMessage[] toDelete = _userMessages
                .Where(um => um.User == user)
                .ToArray();
            
            foreach (UserMessage message in toDelete)
                _userMessages.Remove(message);

            return Task.CompletedTask;
        }
    }
}
