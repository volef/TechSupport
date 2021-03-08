using Microsoft.Extensions.DependencyInjection;
using Web.Services.SupportIdentityManagers;
using Web.Services.SupportRequestRepository;

namespace Web.Services
{
    public static class ServiceExtend
    {
        public static IServiceCollection AddSupportRequestRepository(this IServiceCollection services)
        {
            return services.AddScoped<ISupportRequestRepository, SupportRequestRepository.SupportRequestRepository>();
        }

        public static IServiceCollection AddSupportRequestManager(this IServiceCollection services)
        {
            return services.AddScoped<SupportRequestManager>();
        }

        public static IServiceCollection AddSupportIdentityManager(this IServiceCollection services)
        {
            return services.AddScoped<ISupportIdentityManager, SupportIdentityManager>();
        }

        public static IServiceCollection AddSupportRequestQueue(this IServiceCollection services)
        {
            return services.AddSingleton<SupportRequestQueue.SupportRequestQueue>();
        }
    }
}