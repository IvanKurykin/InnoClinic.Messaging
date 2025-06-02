namespace InnoClinic.Messaging.Abstractions;

public interface IMessagePublisher
{
    Task PublishEntityUpdated<T>(T payload, CancellationToken cancellationToken = default) where T : class;
    Task PublishEntityCreated<T>(T payload,CancellationToken cancellationToken = default) where T : class;
    Task PublishEntityDeleted<T>(T payload, string id, CancellationToken cancellationToken = default) where T : class;
}