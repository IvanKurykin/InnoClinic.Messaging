namespace InnoClinic.Messaging.Interfaces;

public interface IEventPublisher
{
    void Publish<TEvent>(TEvent @event, string? routingKey = null);
}