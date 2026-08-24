using RabbitMQ.Client;

namespace CashFlow.Infrastructure.Messaging;

public static class RabbitMqTopology
{
    public const string ExchangeName = "cashflow.lancamentos";
    public const string QueueName = "lancamento-registrado";
    public const string RoutingKey = "lancamento.registrado";

    public const string DeadLetterExchangeName = "cashflow.lancamentos.dlx";
    public const string DeadLetterQueueName = "lancamento-registrado.dlq";
    public const string DeadLetterRoutingKey = "lancamento.registrado.dlq";

    public static async Task DeclareAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: DeadLetterExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: DeadLetterQueueName,
            exchange: DeadLetterExchangeName,
            routingKey: DeadLetterRoutingKey,
            cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var queueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = DeadLetterExchangeName,
            ["x-dead-letter-routing-key"] = DeadLetterRoutingKey
        };

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            cancellationToken: cancellationToken);
    }
}
