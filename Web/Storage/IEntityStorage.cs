using Web.Entity;

namespace Web.Storage
{
    public interface IEntityStorage
    {
        List<UserMessage> UserMessages { get; }

        List<DialogUser> Users { get; }
    }
}
