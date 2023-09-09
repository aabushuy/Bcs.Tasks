using Web.Entity;

namespace Web.Storage
{
    public class MemoryStorage : IEntityStorage
    {
        public List<UserMessage> UserMessages { get; } = new();
        
        public List<DialogUser> Users { get; } = new();
    }
}
