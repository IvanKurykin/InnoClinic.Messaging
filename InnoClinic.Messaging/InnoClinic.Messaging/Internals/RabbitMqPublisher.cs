using InnoClinic.Messaging.Configurations;
using InnoClinic.Messaging.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using InnoClinic.Messaging.Internals;
using Microsoft.Extensions.Options;

namespace InnoClinic.Messaging.Services;

internal sealed class RabbitMqPublisher : IEventPublisher
{
    private readonly RabbitMqConnection _connection;
    private readonly ExchangeOptions _exchangeOptions;

    public RabbitMqPublisher(RabbitMqConnection connection, IOptions<ExchangeOptions> exchangeOptions)
    {
        _connection = connection;
        _exchangeOptions = exchangeOptions.Value;
        DeclareExchange();
    }

    private void DeclareExchange()
    {
        _connection.Channel.ExchangeDeclare(
            exchange: _exchangeOptions.Name,
            type: _exchangeOptions.Type,
            durable: _exchangeOptions.Durable,
            autoDelete: _exchangeOptions.AutoDelete);
    }

    public void Publish<TEvent>(TEvent @event, string? routingKey = null)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);
        var properties = _connection.Channel.CreateBasicProperties();
        properties.Persistent = true;

        _connection.Channel.BasicPublish(
            exchange: _exchangeOptions.Name,
            routingKey: routingKey ?? string.Empty,
            basicProperties: properties,
            body: body);
    }
}