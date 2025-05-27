namespace InnoClinic.Messaging.Configurations;

public class AzureServiceBusOptions
{
    public const string SectionName = "AzureServiceBusOptions";

    public string ConnectionString { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public bool DlqEnabled { get; set; }
}