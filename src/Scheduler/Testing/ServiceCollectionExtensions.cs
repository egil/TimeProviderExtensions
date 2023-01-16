using Microsoft.Extensions.DependencyInjection;

namespace Scheduler.Testing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestScheduler(this IServiceCollection services)
    {
        services.AddSingleton<ITimeScheduler, TestScheduler>();
        return services;
    }
}
