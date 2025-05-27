using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Abstractions.Events;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Infrastructure.Events;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly IEventSerializer _eventSerializer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(
        IConfiguration configuration,
        IEventSerializer eventSerializer,
        ILogger<KafkaProducer> logger
    )
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration.GetConnectionString("Kafka"),
            Acks = Acks.All, // Wait for all replicas
            RetryBackoffMs = 1000,
            EnableIdempotence = true, // Prevent duplicates
            MessageSendMaxRetries = 3, // Número de reintentos si falla el envío
            CompressionType = CompressionType.Snappy,
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => logger.LogError("Kafka error: {Error}", e.Reason))
            .Build();

        _eventSerializer = eventSerializer;
        _logger = logger;
    }

    public async Task PublishAsync<T>(
        string topic,
        T message,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        await PublishAsync(topic, Guid.NewGuid().ToString(), message, cancellationToken);
    }

    public async Task PublishAsync<T>(
        string topic,
        string key,
        T message,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        try
        {
            var serializedMessage = _eventSerializer.Serialize((IDomainEvent)message);

            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = serializedMessage,
                Headers = new Headers
                {
                    { "EventType", System.Text.Encoding.UTF8.GetBytes(typeof(T).Name) },
                    {
                        "Timestamp",
                        System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O"))
                    },
                    { "Source", System.Text.Encoding.UTF8.GetBytes("TechMart.Auth") },
                },
            };

            var result = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

            _logger.LogDebug(
                "Message published to Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                result.Topic,
                result.Partition.Value,
                result.Offset.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to Kafka topic {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
