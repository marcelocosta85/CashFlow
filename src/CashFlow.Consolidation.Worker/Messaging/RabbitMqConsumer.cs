using System.Text.Json;
using CashFlow.Application.Consolidacao.Commands;
using CashFlow.Domain.Eventos;
using CashFlow.Infrastructure.Messaging;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CashFlow.Consolidation.Worker.Messaging;

public class RabbitMqConsumer : BackgroundService
{
    private static readonly TimeSpan ReconexaoDelay = TimeSpan.FromSeconds(5);

    private readonly RabbitMqConnectionManager _connectionManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly ResiliencePipeline _resiliencePipeline;
    private IChannel? _channel;

    public RabbitMqConsumer(
        RabbitMqConnectionManager connectionManager,
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMqConsumer> logger)
    {
        _connectionManager = connectionManager;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _resiliencePipeline = ConstruirPipelineDeResiliencia(logger);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var conexao = await _connectionManager.ObterConexaoAsync(stoppingToken);
            if (conexao is null)
            {
                _logger.LogWarning("RabbitMQ indisponível — nova tentativa de conexão em {Delay}.", ReconexaoDelay);
                await Task.Delay(ReconexaoDelay, stoppingToken);
                continue;
            }

            try
            {
                _channel = await conexao.CreateChannelAsync(cancellationToken: stoppingToken);
                await RabbitMqTopology.DeclareAsync(_channel, stoppingToken);
                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (_, ea) => await ProcessarMensagemAsync(ea, stoppingToken);

                await _channel.BasicConsumeAsync(RabbitMqTopology.QueueName, autoAck: false, consumer, stoppingToken);

                while (_channel.IsOpen && !stoppingToken.IsCancellationRequested)
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consumir a fila do RabbitMQ — nova tentativa em {Delay}.", ReconexaoDelay);
                await Task.Delay(ReconexaoDelay, stoppingToken);
            }
        }
    }

    private async Task ProcessarMensagemAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
    {
        try
        {
            await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                var evento = JsonSerializer.Deserialize<LancamentoRegistradoEvent>(ea.Body.Span)
                    ?? throw new InvalidOperationException("Mensagem inválida: não foi possível desserializar o evento LancamentoRegistradoEvent.");

                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var comando = new ConsolidarLancamentoCommand(evento.LancamentoId, evento.Tipo, evento.Valor, evento.Data);
                var aplicado = await sender.Send(comando, ct);

                if (!aplicado)
                    _logger.LogInformation("Lançamento {LancamentoId} já havia sido processado — mensagem duplicada ignorada.", evento.LancamentoId);
            }, stoppingToken);

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogWarning(ex, "Circuito de processamento aberto — mensagem recolocada na fila para nova tentativa posterior.");
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao processar mensagem após todas as tentativas — enviando para a dead-letter queue.");
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
        }
    }

    private static ResiliencePipeline ConstruirPipelineDeResiliencia(ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(
                    ex => ex is not OperationCanceledException and not BrokenCircuitException),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                OnRetry = args =>
                {
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Tentativa {Tentativa} de processar mensagem falhou. Nova tentativa em {Delay}.",
                        args.AttemptNumber + 1, args.RetryDelay);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(ex => ex is not OperationCanceledException),
                FailureRatio = 0.5,
                MinimumThroughput = 4,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(15),
                OnOpened = args =>
                {
                    logger.LogError(
                        "Circuito de processamento de mensagens aberto — pausando por {BreakDuration}.",
                        args.BreakDuration);
                    return ValueTask.CompletedTask;
                },
                OnClosed = _ =>
                {
                    logger.LogInformation("Circuito de processamento de mensagens fechado — processamento normalizado.");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            await _channel.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}
