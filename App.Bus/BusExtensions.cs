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
                x.AddConsumer<ProductDeletedEventConsumer>();
                x.AddConsumer<CategoryAddedEventConsumer>();
                x.AddConsumer<CategoryDeletedEventConsumer>();

                x.AddConsumer<ReserveProductConsumer>();
                x.AddConsumer<ReserveCategoryConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(serviceBusOptions!.Url), h =>
                    {

                    });
                    
                    cfg.ReceiveEndpoint(ServiceBusConst.ProductAddedEventQueueName,
                        e =>
                        {
                            e.ConfigureConsumer<ProductAddedEventConsumer>(context);
                        });
                    cfg.ReceiveEndpoint(ServiceBusConst.ProductDeletedEventQueueName,
                        e =>
                        {
                            e.ConfigureConsumer<ProductDeletedEventConsumer>(context);
                        });
                    //for category
                    cfg.ReceiveEndpoint(ServiceBusConst.CategoryAddedEventQueueName, e =>
                    {
                        e.ConfigureConsumer<CategoryAddedEventConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(ServiceBusConst.CategoryDeletedEventQueueName,
                        e =>
                        {
                            e.ConfigureConsumer<CategoryDeletedEventConsumer>(context);
                        });

                    cfg.ReceiveEndpoint(ServiceBusConst.ReserveProductCommandQueueName, e =>
                    {
                        e.ConfigureConsumer<ReserveProductConsumer>(context);

                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    });
                    cfg.ReceiveEndpoint(ServiceBusConst.ReserveCategoryCommandQueueName, e =>
                    {
                    e.ConfigureConsumer<ReserveCategoryConsumer>(context);

                    e.UseMessageRetry(r =>r.Interval(3, TimeSpan.FromSeconds(5)));
                    });

                });
            });
        }
    }
}
