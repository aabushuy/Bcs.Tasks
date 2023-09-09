using Web.Entity;

namespace Web.Repository
{
    public interface IUserRepository
    {
        Task<DialogUser?> GetUser(int id);

        Task<DialogUser> GetOrAddUser(string name);

        Task DeleteUser(int id);
    }
}
