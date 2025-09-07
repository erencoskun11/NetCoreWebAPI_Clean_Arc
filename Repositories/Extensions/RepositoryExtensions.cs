using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace App.Repositories.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionStrings = configuration.GetSection
                (ConnectionStringOptions.Key).Get<ConnectionStringOptions>();
                options.UseNpgsql(connectionStrings!.PostgreSql,postsqlOptionsAction =>
                {
                    postsqlOptionsAction.MigrationsAssembly(typeof(RepositoryAssambly).Assembly.FullName);
                });
            });
            return services;
        }
    }
}
