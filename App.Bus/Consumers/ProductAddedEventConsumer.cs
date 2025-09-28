using App.Domain.Events.ProductEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace App.Bus.Consumers
{
    public class ProductAddedEventConsumer : IConsumer<ProductAddedEvent>
    {
        private readonly ILogger<ProductAddedEventConsumer> _logger;

        public ProductAddedEventConsumer(ILogger<ProductAddedEventConsumer> logger) => _logger = logger;
        public Task Consume(ConsumeContext<ProductAddedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation($"Received Event: {msg.Id} - {msg.Name} - {msg.Price}");
        return Task.CompletedTask;      
         }
    }
}
