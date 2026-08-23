using CashFlow.Domain.Entidades;
using CashFlow.Domain.Enums;
using FluentAssertions;

namespace CashFlow.Domain.Tests.Entidades;

public class SaldoDiarioTests
{
    [Fact]
    public void AplicarLancamento_DeveSomarNoTotalDeCreditos_QuandoTipoForCredito()
    {
        var saldo = new SaldoDiario(DateTime.UtcNow.Date);

        saldo.AplicarLancamento(TipoLancamento.Credito, 100m);
        saldo.AplicarLancamento(TipoLancamento.Credito, 50m);

        saldo.TotalCreditos.Should().Be(150m);
        saldo.TotalDebitos.Should().Be(0m);
        saldo.SaldoConsolidado.Should().Be(150m);
    }

    [Fact]
    public void AplicarLancamento_DeveSomarNoTotalDeDebitos_QuandoTipoForDebito()
    {
        var saldo = new SaldoDiario(DateTime.UtcNow.Date);

        saldo.AplicarLancamento(TipoLancamento.Debito, 30m);

        saldo.TotalDebitos.Should().Be(30m);
        saldo.TotalCreditos.Should().Be(0m);
        saldo.SaldoConsolidado.Should().Be(-30m);
    }

    [Fact]
    public void SaldoConsolidado_DeveSerDiferencaEntreCreditosEDebitos()
    {
        var saldo = new SaldoDiario(DateTime.UtcNow.Date);

        saldo.AplicarLancamento(TipoLancamento.Credito, 200m);
        saldo.AplicarLancamento(TipoLancamento.Debito, 80m);

        saldo.SaldoConsolidado.Should().Be(120m);
    }

    [Fact]
    public void SaldoConsolidado_DeveSerZero_QuandoNaoHouverLancamentos()
    {
        var saldo = new SaldoDiario(DateTime.UtcNow.Date);

        saldo.SaldoConsolidado.Should().Be(0m);
    }
}
