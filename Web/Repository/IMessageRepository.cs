using Web.Entity;

namespace Web.Repository
{
    public interface IMessageRepository
    {
        Task<IEnumerable<UserMessage>> GetAll();

        Task<IEnumerable<UserMessage>> GetMessagesByUserId(int id);

        Task Add(UserMessage chatMessage);
    }
}
