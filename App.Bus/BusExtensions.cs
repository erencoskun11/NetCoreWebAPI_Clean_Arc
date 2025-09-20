using App.Domain.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace App.Bus
{
    public static class BusExtensions
    {
        public static void AddBus(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceBusOptions = configuration
                .GetSection(nameof(ServiceBusOptions))
                .Get<ServiceBusOptions>();

            services.AddMassTransit(x =>
            {
                // Eğer consumer’ların varsa burada ekleyebilirsin
                // x.AddConsumer<ProductCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(serviceBusOptions!.Url), h =>
                    {
                       
                    });

                    // Consumer’ları otomatik ekle
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
