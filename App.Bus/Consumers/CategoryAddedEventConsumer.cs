using App.Domain.Events.CategoryEvents;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace App.Bus.Consumers
{
    public class CategoryAddedEventConsumer : IConsumer<CategoryAddedEvent>
    {
        private readonly ILogger<CategoryAddedEventConsumer> _logger;
        public CategoryAddedEventConsumer(ILogger<CategoryAddedEventConsumer> logger) { _logger = logger; }
        public Task Consume(ConsumeContext<CategoryAddedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation($"Received event : {msg.Id} - {msg.name} - {msg.Created} - {msg.Updated}");
        return Task.CompletedTask;      
        }
    }
}
