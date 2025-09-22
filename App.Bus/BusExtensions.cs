using App.Application.Contracts.ServiceBus;
using App.Bus.Consumers;
using App.Domain.Const;
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

            services.AddScoped<IServiceBus,ServiceBus>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ProductAddedEventConsumer>();
                x.AddConsumer<CategoryAddedEventConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(serviceBusOptions!.Url), h =>
                    {
                       
                    });
                    //for product
                    cfg.ReceiveEndpoint(ServiceBusConst.ProductAddedEventQueueName,
                        e =>
                        {
                            e.ConfigureConsumer<ProductAddedEventConsumer>(context);
                        });
                    //for category
                    cfg.ReceiveEndpoint(ServiceBusConst.CategoryAddedEventQueueName, e =>
                    {
                        e.ConfigureConsumer<CategoryAddedEventConsumer>(context);
                    });
                });
            });
        }
    }
}
