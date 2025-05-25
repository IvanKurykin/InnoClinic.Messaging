using InnoClinic.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;

namespace InnoClinic.Messaging.Internals;

public class RabbitMqBackgroundService(IEventConsumer consumer) : BackgroundService
{
    private readonly IEventConsumer _consumer = consumer;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.StartConsuming();
        return Task.CompletedTask;
    }
}