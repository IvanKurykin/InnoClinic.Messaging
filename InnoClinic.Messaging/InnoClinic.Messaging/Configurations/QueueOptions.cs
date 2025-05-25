namespace InnoClinic.Messaging.Configurations;

public class QueueOptions
{
    public string Name { get; set; } = null!;
    public List<string> RoutingKeys { get; set; } = [];
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public bool Lazy { get; set; } = false;
    public bool DlqEnabled { get; set; } = true;
}