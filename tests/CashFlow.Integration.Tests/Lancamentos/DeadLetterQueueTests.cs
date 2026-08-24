using System.Text;
using CashFlow.Infrastructure.Messaging;
using CashFlow.Integration.Tests.Infrastructure;
using FluentAssertions;
using RabbitMQ.Client;

namespace CashFlow.Integration.Tests.Lancamentos;

/// <summary>
/// T31 — RF02: falha persistente no processamento do consumer não pode perder a mensagem.
/// Publica uma mensagem que nunca conseguirá ser desserializada (falha determinística),
/// esgotando o retry do Polly (3 tentativas) e confirmando o roteamento para a dead-letter queue.
/// </summary>
public class DeadLetterQueueTests : IAsyncLifetime
{
    private readonly CashFlowIntegrationTestFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Falha_Persistente_No_Consumer_Deve_Enviar_Mensagem_Para_Dead_Letter_Queue()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var connectionFactory = new ConnectionFactory
        {
            HostName = _fixture.RabbitMqHostName,
            Port = _fixture.RabbitMqPort,
            UserName = _fixture.RabbitMqUserName,
            Password = _fixture.RabbitMqPassword
        };

        await using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await RabbitMqTopology.DeclareAsync(channel, cancellationToken);

        var corpoInvalido = Encoding.UTF8.GetBytes("{ isto não é um LancamentoRegistradoEvent válido }");

        await channel.BasicPublishAsync(
            exchange: RabbitMqTopology.ExchangeName,
            routingKey: RabbitMqTopology.RoutingKey,
            mandatory: false,
            basicProperties: new BasicProperties { Persistent = true, ContentType = "application/json" },
            body: corpoInvalido,
            cancellationToken: cancellationToken);

        BasicGetResult? mensagemNaDlq = null;

        // Precisa cobrir o retry exponencial do consumer (1s + 2s + 4s ≈ 7s) antes do NACK final.
        await Espera.AteAsync(async () =>
        {
            mensagemNaDlq = await channel.BasicGetAsync(RabbitMqTopology.DeadLetterQueueName, autoAck: true, cancellationToken);
            return mensagemNaDlq is not null;
        }, timeoutSegundos: 30);

        mensagemNaDlq.Should().NotBeNull();
        Encoding.UTF8.GetString(mensagemNaDlq!.Body.Span).Should().Contain("não é um LancamentoRegistradoEvent válido");
    }
}
