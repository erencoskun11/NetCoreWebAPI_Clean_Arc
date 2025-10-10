using App.Repositories.Categories;
using App.Repositories.Products;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Repositories.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            // open-generic registration: IGenericRepository<TEntity, TId> => GenericRepository<TEntity, TId>
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            // concrete repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
