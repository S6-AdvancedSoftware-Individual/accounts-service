using Application.Common.Interfaces;
using Azure.Messaging.ServiceBus;

public class ServiceBusMessagePublisher : IMessagePublisher
{
    private readonly ServiceBusClient _client;
    private readonly string _topic;
    private readonly ILogger<ServiceBusMessagePublisher> _logger;

    public ServiceBusMessagePublisher(string connectionString, string topic, ILogger<ServiceBusMessagePublisher> logger)
    {
        var clientOptions = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        _client = new ServiceBusClient(connectionString, clientOptions);
        _topic = topic;
        _logger = logger;
    }

    public async Task PublishMessagesAsync(IEnumerable<string> messages)
    {
        var sender = _client.CreateSender(_topic);

        using var batch = await sender.CreateMessageBatchAsync();
        foreach (var message in messages)
        {
            if (!batch.TryAddMessage(new ServiceBusMessage(message)))
            {
                throw new InvalidOperationException($"Message too large: {message}");
            }
        }

        await sender.SendMessagesAsync(batch);
        _logger.LogInformation("Sent {Count} messages to topic {Topic}", messages.Count(), _topic);
    }
}
