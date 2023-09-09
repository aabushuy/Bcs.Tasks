using Web.Repository;
using Web.Storage;

namespace Web.Extentions
{
    public static class DI
    {
        public static IServiceCollection AddDependecyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IEntityStorage, MemoryStorage>();

            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}
