using System.Text;
using System.Text.Json;
using CashFlow.Application.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CashFlow.Infrastructure.Messaging;

public class RabbitMqPublisher : IEventPublisher
{
    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(RabbitMqConnectionManager connectionManager, ILogger<RabbitMqPublisher> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task PublicarAsync<TEvento>(TEvento evento, CancellationToken cancellationToken) where TEvento : class
    {
        try
        {
            var connection = await _connectionManager.ObterConexaoAsync(cancellationToken);
            if (connection is null)
            {
                _logger.LogWarning("RabbitMQ indisponível — evento {EventoTipo} não foi publicado.", typeof(TEvento).Name);
                return;
            }

            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await RabbitMqTopology.DeclareAsync(channel, cancellationToken);

            var corpo = JsonSerializer.SerializeToUtf8Bytes(evento);

            var propriedades = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json"
            };

            await channel.BasicPublishAsync(
                exchange: RabbitMqTopology.ExchangeName,
                routingKey: RabbitMqTopology.RoutingKey,
                mandatory: false,
                basicProperties: propriedades,
                body: corpo,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao publicar evento {EventoTipo} no RabbitMQ.", typeof(TEvento).Name);
        }
    }
}
