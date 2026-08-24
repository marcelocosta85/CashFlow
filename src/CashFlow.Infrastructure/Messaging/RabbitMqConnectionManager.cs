using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CashFlow.Infrastructure.Messaging;

public sealed class RabbitMqConnectionManager : IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnection? _connection;

    public RabbitMqConnectionManager(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnectionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IConnection?> ObterConexaoAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            return _connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Não foi possível conectar ao RabbitMQ em {HostName}:{Port}.", _options.HostName, _options.Port);
            return null;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
    }
}
