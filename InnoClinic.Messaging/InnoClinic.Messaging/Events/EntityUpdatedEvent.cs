namespace InnoClinic.Messaging.Events.EntityUpdatedEvent;

public record EntityUpdatedEvent<T> : IEntityEvent where T : class
{
    public string EntityType => typeof(T).Name;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public required T Payload { get; set; }
}