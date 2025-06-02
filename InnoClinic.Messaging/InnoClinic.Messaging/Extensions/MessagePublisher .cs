using InnoClinic.Messaging.Abstractions;
using InnoClinic.Messaging.Events;
using InnoClinic.Messaging.Events.EntityUpdatedEvent;
using MassTransit;

namespace InnoClinic.Messaging.Extensions;

internal class MessagePublisher : IMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MessagePublisher(IPublishEndpoint publishEndpoint) => _publishEndpoint = publishEndpoint;

    public async Task PublishEntityUpdated<T>(T payload, CancellationToken cancellationToken = default) where T : class
    {
        var entityName = typeof(T).Name.ToLower();
        var eventMessage = new EntityUpdatedEvent<T> { Payload = payload };

        await _publishEndpoint.Publish(eventMessage, ctx => ctx.SetRoutingKey($"{entityName}.updated"), cancellationToken);
    }

    public async Task PublishEntityCreated<T>(T payload, CancellationToken cancellationToken = default) where T : class
    {
        var entityName = typeof(T).Name.ToLower();
        var eventMessage = new EntityCreatedEvent<T> { Payload = payload };

        await _publishEndpoint.Publish(eventMessage, ctx => ctx.SetRoutingKey($"{entityName}.created"), cancellationToken);
    }

    public async Task PublishEntityDeleted<T>(T payload, string id, CancellationToken cancellationToken = default) where T : class
    {
        var entityName = typeof(T).Name.ToLower();
        var eventMessage = new EntityDeletedEvent<T> { Payload = payload, Id = id };

        await _publishEndpoint.Publish(eventMessage, ctx => ctx.SetRoutingKey($"{entityName}.deleted"), cancellationToken);
    }
}