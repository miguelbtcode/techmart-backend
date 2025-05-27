namespace TechMart.Auth.Infrastructure.Events;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default)
        where T : class;
    Task PublishAsync<T>(
        string topic,
        string key,
        T message,
        CancellationToken cancellationToken = default
    )
        where T : class;
}
