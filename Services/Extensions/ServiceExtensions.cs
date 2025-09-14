using App.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Services.Products;
using FluentValidation;
using App.Services.Products.Create;
using AutoMapper;
using Microsoft.Extensions.Logging;
using App.Services.ExceptionHandlers;
using App.Services.Categories;

namespace App.Services.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProductService, ProductService>();
            
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // FluentValidation validator'larını register et
            services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

            // AutoMapper - IMapper'ı service provider üzerinden oluşturup register ediyoruz
            services.AddSingleton<IMapper>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                var config = new MapperConfiguration(cfg =>
                {
                    // Profilini burada ekle (veya cfg.AddMaps(...) kullan)
                    cfg.AddProfile(new CategoryProfileMapping());

                    cfg.AddProfile(new ProductsMappingProfile());
                }, loggerFactory);

                // Validate configuration (opsiyonel ama tavsiye edilir)
                config.AssertConfigurationIsValid();

                return config.CreateMapper();
            });

            services.AddExceptionHandler<CriticalExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();




            return services;
        }
    }
}
