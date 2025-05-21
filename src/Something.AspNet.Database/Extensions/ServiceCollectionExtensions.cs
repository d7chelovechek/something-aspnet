using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;
using Something.AspNet.Database.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Something.AspNet.Database.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddOptions<DatabaseOptions>().BindConfiguration(nameof(DatabaseOptions));

            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();

            return services; 
        }
    }
}