using InnoClinic.Messaging.Configurations;
using InnoClinic.Messaging.Interfaces;
using InnoClinic.Messaging.Internals;
using InnoClinic.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace InnoClinic.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddSingleton(provider =>
        {
            var options = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()!;
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                DispatchConsumersAsync = false
            };
            return new RabbitMqConnection(factory);
        });

        services.AddSingleton<IEventPublisher, RabbitMqPublisher>();
        services.AddSingleton<IEventConsumer, RabbitMqConsumer>();
        services.AddHostedService<RabbitMqBackgroundService>();

        return services;
    }
}