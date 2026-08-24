using System.Net.Http.Json;
using CashFlow.Application.Abstractions;
using CashFlow.Application.Consolidacao.Queries;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Eventos;
using CashFlow.Integration.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Integration.Tests.Lancamentos;

/// <summary>
/// T30 — RF02: reprocessar a mesma mensagem (mesmo LancamentoId) não pode duplicar o valor
/// no saldo. Simula redelivery do broker publicando o mesmo evento duas vezes.
/// </summary>
public class IdempotenciaConsolidacaoTests : IAsyncLifetime
{
    private readonly CashFlowIntegrationTestFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Mensagem_Duplicada_Nao_Deve_Duplicar_Saldo()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var data = DateTime.UtcNow.Date.AddDays(-2);
        var evento = new LancamentoRegistradoEvent(Guid.NewGuid(), TipoLancamento.Debito, 40m, data, "Pagamento fornecedor");

        var publisher = _fixture.LaunchesApiFactory.Services.GetRequiredService<IEventPublisher>();

        await publisher.PublicarAsync(evento, cancellationToken);
        await publisher.PublicarAsync(evento, cancellationToken);

        var workerClient = _fixture.ConsolidationWorkerFactory.CreateClient();

        SaldoDiarioResultado? resultado = null;

        await Espera.AteAsync(async () =>
        {
            var response = await workerClient.GetAsync($"/saldos/{data:yyyy-MM-dd}", cancellationToken);
            response.EnsureSuccessStatusCode();
            resultado = await response.Content.ReadFromJsonAsync<SaldoDiarioResultado>(cancellationToken);
            return resultado is not null && resultado.TotalDebitos == 40m;
        });

        // Aguarda mais um pouco: se a segunda mensagem fosse (indevidamente) reaplicada,
        // já teria acontecido nesse intervalo adicional.
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        var respostaFinal = await workerClient.GetAsync($"/saldos/{data:yyyy-MM-dd}", cancellationToken);
        var saldoFinal = await respostaFinal.Content.ReadFromJsonAsync<SaldoDiarioResultado>(cancellationToken);

        saldoFinal!.TotalDebitos.Should().Be(40m);
        saldoFinal.SaldoConsolidado.Should().Be(-40m);
    }
}
