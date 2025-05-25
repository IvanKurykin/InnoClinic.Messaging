namespace InnoClinic.Messaging.Events;

public record EntityUpdatedEvent<T>
{
    public Guid Id { get; init; }
    public string? EntityType { get; init; } 
    public DateTime UpdatedAt { get; init; }
    public T? Payload { get; init; }
}