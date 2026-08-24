using System.Net.Http.Json;
using CashFlow.Application.Consolidacao.Queries;
using CashFlow.Domain.Enums;
using CashFlow.Integration.Tests.Infrastructure;
using CashFlow.Launches.Api.Contracts;
using FluentAssertions;

namespace CashFlow.Integration.Tests.Lancamentos;

/// <summary>
/// T29 — fluxo ponta a ponta: POST /lancamentos na Launches.Api publica o evento na fila,
/// o Consolidation.Worker consome de forma assíncrona e GET /saldos/{data} reflete o valor.
/// </summary>
public class RegistrarLancamentoEndToEndTests : IAsyncLifetime
{
    private readonly CashFlowIntegrationTestFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Deve_Consolidar_Saldo_Diario_Apos_Registrar_Lancamento()
    {
        var data = DateTime.UtcNow.Date.AddDays(-1);
        var apiClient = _fixture.LaunchesApiFactory.CreateClient();
        var workerClient = _fixture.ConsolidationWorkerFactory.CreateClient();

        var cancellationToken = TestContext.Current.CancellationToken;
        var request = new RegistrarLancamentoRequest(TipoLancamento.Credito, 150.75m, data, "Venda balcão");

        var response = await apiClient.PostAsJsonAsync("/lancamentos", request, cancellationToken);
        response.IsSuccessStatusCode.Should().BeTrue();

        var criado = await response.Content.ReadFromJsonAsync<LancamentoCriadoResponse>(cancellationToken);
        criado.Should().NotBeNull();
        criado!.Id.Should().NotBeEmpty();

        SaldoDiarioResultado? resultado = null;

        await Espera.AteAsync(async () =>
        {
            var saldoResponse = await workerClient.GetAsync($"/saldos/{data:yyyy-MM-dd}", cancellationToken);
            saldoResponse.EnsureSuccessStatusCode();
            resultado = await saldoResponse.Content.ReadFromJsonAsync<SaldoDiarioResultado>(cancellationToken);
            return resultado is not null && resultado.TotalCreditos == 150.75m;
        });

        resultado!.TotalCreditos.Should().Be(150.75m);
        resultado.TotalDebitos.Should().Be(0m);
        resultado.SaldoConsolidado.Should().Be(150.75m);
    }
}
