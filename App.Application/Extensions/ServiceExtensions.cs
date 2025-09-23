using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore; // sadece validator registration için (opsiyonel)

using App.Application.Features.Categories;
using App.Application.Features.Products;

namespace App.Application.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Application katmanına özgü servis kayıtları (web-özgü ayarlar burada olmamalı).
        /// - Servis arayüzleri / implementasyonları
        /// - AutoMapper profil registration (tip bazlı, assembly yerine tip veriyoruz)
        /// - FluentValidation validator'larını register et (opsiyonel)
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Domain / application servisleri
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();

            // AutoMapper: profil tiplerini doğrudan veriyoruz.
            // NOT: Burada kullandığımız tiplerin proje içinde public ve Profile'dan türemiş olması gerekir.
            services.AddAutoMapper(
                typeof(App.Application.Features.Categories.CategoryProfileMapping),
                typeof(App.Application.Features.Products.ProductsMappingProfile)
            );

            // FluentValidation: validator'ların bulunduğu assembly'yi register et.
            // (Eğer validator'ları API katmanında otomatik validasyonla kullanmak istiyorsan
            // bu kısmı web katmanına taşı.)
            services.AddValidatorsFromAssembly(typeof(App.Application.Features.Products.Create.CreateProductRequestValidator).Assembly);

            return services;
        }
    }
}
