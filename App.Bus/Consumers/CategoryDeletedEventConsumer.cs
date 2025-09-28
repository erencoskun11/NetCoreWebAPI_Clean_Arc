using App.Domain.Events.CategoryEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Bus.Consumers
{
    public class CategoryDeletedEventConsumer : IConsumer<CategoryDeletedEvent>
    {
        private readonly ILogger<CategoryDeletedEventConsumer> _logger;
        public CategoryDeletedEventConsumer (ILogger<CategoryDeletedEventConsumer> logger) =>_logger = logger;
        public Task Consume(ConsumeContext<CategoryDeletedEvent> context)
        {
            var msg = context.Message;
            _logger.LogInformation($"received event : {msg.Id} - {msg.Name} ");
            return Task.CompletedTask;
        }
    }
}
