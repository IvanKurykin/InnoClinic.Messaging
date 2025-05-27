namespace InnoClinic.Messaging.Configurations;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMqOptions";

    public string HostName { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}