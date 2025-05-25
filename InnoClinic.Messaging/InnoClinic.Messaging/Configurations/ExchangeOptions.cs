namespace InnoClinic.Messaging.Configurations;

public class ExchangeOptions
{
    public string Name { get; set; } = "event_bus";
    public string Type { get; set; } = "direct";
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; } = false;
}