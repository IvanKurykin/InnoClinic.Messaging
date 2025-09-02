using InnoClinic.Messaging.Abstractions;
using InnoClinic.Messaging.Configurations;
using InnoClinic.Messaging.Enums;
using InnoClinic.Messaging.Events.EntityUpdatedEvent;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InnoClinic.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    private static readonly List<Action<IRabbitMqBusFactoryConfigurator>> _rabbitMqPublishTopologyConfigs = new();
    private static readonly List<Action<IServiceBusBusFactoryConfigurator>> _azurePublishTopologyConfigs = new();

    private static bool _isMassTransitConfigured = false;

    public static IServiceCollection AddMessagingForEntityType<TEntity>(
        this IServiceCollection services,
        MessageBrokerType brokerType,
        bool configureConsumersForEntityType = false) 
        where TEntity : class
    {
        if (_isMassTransitConfigured)
        {
            Console.WriteLine($"Warning: AddMessagingForEntityType<{typeof(TEntity).Name}> called after MassTransit was configured. This call might be ignored for topology settings.");
        }

        services.TryAddScoped<IMessagePublisher, MessagePublisher>();

        switch (brokerType)
        {
            case MessageBrokerType.RabbitMQ:
                _rabbitMqPublishTopologyConfigs.Add(cfg =>
                {
                    cfg.Publish<EntityUpdatedEvent<TEntity>>(p =>
                    { p.ExchangeType = Configurations.RabbitMqExchangeTypes.Topic; });
                });
                break;

            case MessageBrokerType.AzureServiceBus:
                _azurePublishTopologyConfigs.Add(cfg =>
                { });
                break;
        }
        return services;
    }

    public static IServiceCollection AddDefaultMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        MessageBrokerType brokerType,
        Action<IBusRegistrationConfigurator>? consumerRegistrationAction = null)
    {
        if (_isMassTransitConfigured)
        {
            Console.WriteLine("Warning: AddDefaultMassTransit called more than once.");
            return services;
        }

        services.TryAddScoped<IMessagePublisher, MessagePublisher>(); 

        services.AddMassTransit(x =>
        {
            consumerRegistrationAction?.Invoke(x);

            switch (brokerType)
            {
                case MessageBrokerType.RabbitMQ:
                    var rabbitMqOptions = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()
                                        ?? throw new ArgumentNullException(nameof(RabbitMqOptions), $"{RabbitMqOptions.SectionName} was not found in configuration.");

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(rabbitMqOptions.HostName, rabbitMqOptions.VirtualHost, h =>
                        {
                            h.Username(rabbitMqOptions.UserName);
                            h.Password(rabbitMqOptions.Password);
                        });

                        foreach (var configAction in _rabbitMqPublishTopologyConfigs)
                        {
                            configAction(cfg);
                        }

                        cfg.ConfigureEndpoints(context);
                    });
                    break;

                case MessageBrokerType.AzureServiceBus:
                    var azureOptions = configuration.GetSection(AzureServiceBusOptions.SectionName).Get<AzureServiceBusOptions>()
                                     ?? throw new ArgumentNullException(nameof(AzureServiceBusOptions), $"{AzureServiceBusOptions.SectionName} was not found in configuration.");

                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        cfg.Host(azureOptions.ConnectionString);

                        foreach (var configAction in _azurePublishTopologyConfigs)
                        {
                            configAction(cfg);
                        }

                        cfg.ConfigureEndpoints(context);
                    });
                    break;
            }
        });

        _isMassTransitConfigured = true;
        _rabbitMqPublishTopologyConfigs.Clear(); 
        _azurePublishTopologyConfigs.Clear();

        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<AzureServiceBusOptions>(configuration.GetSection(AzureServiceBusOptions.SectionName));

        return services;
    }
}
