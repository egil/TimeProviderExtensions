using Scheduler;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTimeScheduler(this IServiceCollection services)
    {
        services.AddSingleton<ITimeScheduler, TimeScheduler>();
        return services;
    }
}
