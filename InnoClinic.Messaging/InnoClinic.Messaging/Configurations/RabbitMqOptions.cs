namespace InnoClinic.Messaging.Configurations;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";
    
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public ExchangeOptions Exchange { get; set; } = new();
    public List<QueueOptions> Queues { get; set; } = [];
}