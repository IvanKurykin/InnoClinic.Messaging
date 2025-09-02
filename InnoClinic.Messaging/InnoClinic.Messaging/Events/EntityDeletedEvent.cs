namespace InnoClinic.Messaging.Events;

public record EntityDeletedEvent<T> : IEntityEvent where T : class
{
    public string EntityType => typeof(T).Name;
    public DateTime DeletedAt { get; init; } = DateTime.UtcNow;
    public required T Payload { get; set; }
    public required string Id { get; set; }
}