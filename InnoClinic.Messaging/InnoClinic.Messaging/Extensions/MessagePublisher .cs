using InnoClinic.Messaging.Abstractions;
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

        Console.WriteLine($"DEBUG [MessagePublisher]: Attempting to publish EntityUpdatedEvent<{typeof(T).Name}>. RoutingKey: '{entityName}.updated'. Payload Type: {payload?.GetType().FullName}");
        try
        {
            await _publishEndpoint.Publish(eventMessage, ctx => ctx.SetRoutingKey($"{entityName}.updated"), cancellationToken);
            Console.WriteLine($"DEBUG [MessagePublisher]: Publish call for EntityUpdatedEvent<{typeof(T).Name}> COMPLETED (await returned).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR [MessagePublisher]: Exception during publish of EntityUpdatedEvent<{typeof(T).Name}>: {ex}");
            throw; 
        }
    }
}