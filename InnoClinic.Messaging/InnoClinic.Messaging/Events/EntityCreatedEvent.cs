namespace InnoClinic.Messaging.Events;

public record EntityCreatedEvent<T> : IEntityEvent where T : class
{
    public string EntityType => typeof(T).Name;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public required T Payload { get; set; }
}