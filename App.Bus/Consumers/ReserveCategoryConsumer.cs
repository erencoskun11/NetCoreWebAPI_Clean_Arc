using App.Domain.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Bus.Consumers
{
    public class ReserveCategoryConsumer : IConsumer<ReserveCategoryCommand>
    {
        private readonly ILogger<ReserveCategoryConsumer> _logger;
        public ReserveCategoryConsumer(ILogger<ReserveCategoryConsumer> logger) => _logger = logger;
        public Task Consume(ConsumeContext<ReserveCategoryCommand> context)
        {
            var message = context.Message;
            _logger.LogInformation("Reserve request: {Id} x {Name}", message.Id, message.Name);
            return Task.CompletedTask;

        }
    }
}
