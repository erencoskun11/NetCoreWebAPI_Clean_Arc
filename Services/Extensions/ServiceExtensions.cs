using App.Services.Categories;
using App.Services.ExceptionHandlers;
using App.Services.Products;
using App.Services.Products.Create;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Services.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 🔹 Servisler
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // 🔹 FluentValidation
            services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

            // 🔹 Exception Handlers
            services.AddExceptionHandler<CriticalExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
