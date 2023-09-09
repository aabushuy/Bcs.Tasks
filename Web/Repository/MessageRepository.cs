using Web.Entity;
using Web.Storage;

namespace Web.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly List<UserMessage> _userMessages;

        public MessageRepository(IEntityStorage entityStorage)
        {
            _userMessages = entityStorage.UserMessages;
        }

        public Task<IEnumerable<UserMessage>> GetAll() => Task.FromResult(_userMessages.AsEnumerable());

        public Task<IEnumerable<UserMessage>> GetMessagesByUserId(int id) => Task.FromResult(_userMessages.Where(m => m.User.Id == id));

        public Task Add(UserMessage chatMessage)
        {
            _userMessages.Add(chatMessage);

            return Task.CompletedTask;
        }
    }
}
