using RabbitMQ.Client;

namespace InnoClinic.Messaging.Internals;

internal sealed class RabbitMqConnection : IDisposable
{
    private readonly IConnection _connection;
    public IModel Channel { get; }

    public RabbitMqConnection(ConnectionFactory factory)
    {
        _connection = factory.CreateConnection();
        Channel = _connection.CreateModel();
    }

    public void Dispose()
    {
        Channel?.Dispose();
        _connection?.Dispose();
    }
}