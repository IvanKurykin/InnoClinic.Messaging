using InnoClinic.Messaging.Configurations;
using InnoClinic.Messaging.Interfaces;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using InnoClinic.Messaging.Internals;
using Microsoft.Extensions.Options;

namespace InnoClinic.Messaging.Services;

internal sealed class RabbitMqConsumer : IEventConsumer
{
    private readonly RabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;
    private readonly IList<EventingBasicConsumer> _consumers = new List<EventingBasicConsumer>();

    public RabbitMqConsumer(RabbitMqConnection connection, IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _options = options.Value;
        DeclareQueues();
    }

    private void DeclareQueues()
    {
        foreach (var queue in _options.Queues)
        {
            var args = new Dictionary<string, object>();

            if (queue.Lazy) args.Add("x-queue-mode", "lazy");

            if (queue.DlqEnabled)
            {
                var dlqName = $"{queue.Name}.dlq";
                args.Add("x-dead-letter-exchange", _options.Exchange.Name);
                args.Add("x-dead-letter-routing-key", dlqName);

                _connection.Channel.QueueDeclare(
                    queue: dlqName,
                    durable: queue.Durable,
                    exclusive: queue.Exclusive,
                    autoDelete: queue.AutoDelete,
                    arguments: args);
            }

            _connection.Channel.QueueDeclare(
                queue: queue.Name,
                durable: queue.Durable,
                exclusive: queue.Exclusive,
                autoDelete: queue.AutoDelete,
                arguments: args);

            foreach (var routingKey in queue.RoutingKeys)
            {
                _connection.Channel.QueueBind(
                    queue: queue.Name,
                    exchange: _options.Exchange.Name,
                    routingKey: routingKey);
            }
        }
    }

    public void Subscribe<TEvent>(Action<TEvent> handler, string queueName)
    {
        var consumer = new EventingBasicConsumer(_connection.Channel);
        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<TEvent>(body);
                if (message != null) handler(message);
                _connection.Channel.BasicAck(ea.DeliveryTag, false);
            }
            catch
            {
                _connection.Channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _connection.Channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        _consumers.Add(consumer);
    }

    public void StartConsuming() { }
}