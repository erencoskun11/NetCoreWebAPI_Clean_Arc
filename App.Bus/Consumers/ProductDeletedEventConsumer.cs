using App.Domain.Events.ProductEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Bus.Consumers
{
    public class ProductDeletedEventConsumer : IConsumer<ProductDeletedEvent>
    {
        private readonly ILogger<ProductDeletedEventConsumer> _logger;
        public ProductDeletedEventConsumer(ILogger<ProductDeletedEventConsumer> logger) =>_logger = logger;
        public Task Consume(ConsumeContext<ProductDeletedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation($"Received Event: {msg.Id} - {msg.Name} - {msg.Price}");
            return Task.CompletedTask;
        }
    }
}
