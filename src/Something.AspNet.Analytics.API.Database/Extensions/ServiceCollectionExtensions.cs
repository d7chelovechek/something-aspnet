using Microsoft.Extensions.DependencyInjection;
using Something.AspNet.Analytics.API.Database.Options;

namespace Something.AspNet.Analytics.API.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));

        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();

        return services; 
    }
}