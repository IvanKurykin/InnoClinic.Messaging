namespace InnoClinic.Messaging.Interfaces;

public interface IEventConsumer
{
    void Subscribe<TEvent>(Action<TEvent> handler, string queueName);
    void StartConsuming();
}