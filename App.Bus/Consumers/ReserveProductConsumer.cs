using App.Domain.Commands;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace App.Bus.Consumers
{
    public class ReserveProductConsumer : IConsumer<ReserveProductCommand>
    {
        private readonly ILogger<ReserveProductConsumer> _logger;

        public ReserveProductConsumer(ILogger<ReserveProductConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReserveProductCommand> context)
        {
            var msg = context.Message;

            _logger.LogInformation("Reserve request: {Id} {Quantity}", msg.Id, msg.Quantity);

            return Task.CompletedTask;
        }
    }
}
