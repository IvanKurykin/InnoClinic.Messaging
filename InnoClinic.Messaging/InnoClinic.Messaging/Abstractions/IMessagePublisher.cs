namespace InnoClinic.Messaging.Abstractions;

public interface IMessagePublisher
{
    Task PublishEntityUpdated<T>(T payload, CancellationToken cancellationToken = default) where T : class;
}