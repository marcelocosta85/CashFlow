using CashFlow.Application.Abstractions;
using CashFlow.Application.Consolidacao.Commands;
using CashFlow.Application.Consolidacao.Handlers;
using CashFlow.Domain.Entidades;
using CashFlow.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace CashFlow.Application.Tests.Consolidacao;

public class ConsolidarLancamentoHandlerTests
{
    private readonly ISaldoDiarioRepositorio _saldoDiarioRepositorio = Substitute.For<ISaldoDiarioRepositorio>();
    private readonly ConsolidarLancamentoHandler _handler;

    public ConsolidarLancamentoHandlerTests()
    {
        _handler = new ConsolidarLancamentoHandler(_saldoDiarioRepositorio);
    }

    [Fact]
    public async Task Handle_DeveCriarNovoSaldoDiario_QuandoNaoExistirSaldoParaAData()
    {
        var data = DateTime.UtcNow.Date;
        var comando = new ConsolidarLancamentoCommand(Guid.NewGuid(), TipoLancamento.Credito, 100m, data);

        _saldoDiarioRepositorio.LancamentoJaProcessadoAsync(comando.LancamentoId, Arg.Any<CancellationToken>())
            .Returns(false);
        _saldoDiarioRepositorio.ObterPorDataAsync(data, Arg.Any<CancellationToken>())
            .Returns((SaldoDiario?)null);

        await _handler.Handle(comando, CancellationToken.None);

        await _saldoDiarioRepositorio.Received(1).RegistrarConsolidacaoAsync(
            Arg.Is<SaldoDiario>(s => s.Data == data && s.TotalCreditos == 100m),
            comando.LancamentoId,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveAplicarSobreSaldoExistente_QuandoJaHouverSaldoParaAData()
    {
        var data = DateTime.UtcNow.Date;
        var saldoExistente = new SaldoDiario(data);
        saldoExistente.AplicarLancamento(TipoLancamento.Credito, 50m);

        var comando = new ConsolidarLancamentoCommand(Guid.NewGuid(), TipoLancamento.Debito, 20m, data);

        _saldoDiarioRepositorio.LancamentoJaProcessadoAsync(comando.LancamentoId, Arg.Any<CancellationToken>())
            .Returns(false);
        _saldoDiarioRepositorio.ObterPorDataAsync(data, Arg.Any<CancellationToken>())
            .Returns(saldoExistente);

        await _handler.Handle(comando, CancellationToken.None);

        await _saldoDiarioRepositorio.Received(1).RegistrarConsolidacaoAsync(
            Arg.Is<SaldoDiario>(s => s.TotalCreditos == 50m && s.TotalDebitos == 20m),
            comando.LancamentoId,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NaoDeveAplicarNemRegistrar_QuandoLancamentoJaTiverSidoProcessado()
    {
        var comando = new ConsolidarLancamentoCommand(Guid.NewGuid(), TipoLancamento.Credito, 100m, DateTime.UtcNow.Date);

        _saldoDiarioRepositorio.LancamentoJaProcessadoAsync(comando.LancamentoId, Arg.Any<CancellationToken>())
            .Returns(true);

        var resultado = await _handler.Handle(comando, CancellationToken.None);

        resultado.Should().BeFalse();
        await _saldoDiarioRepositorio.DidNotReceive().ObterPorDataAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
        await _saldoDiarioRepositorio.DidNotReceive().RegistrarConsolidacaoAsync(
            Arg.Any<SaldoDiario>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveRetornarTrue_QuandoLancamentoForAplicadoComSucesso()
    {
        var comando = new ConsolidarLancamentoCommand(Guid.NewGuid(), TipoLancamento.Credito, 100m, DateTime.UtcNow.Date);

        _saldoDiarioRepositorio.LancamentoJaProcessadoAsync(comando.LancamentoId, Arg.Any<CancellationToken>())
            .Returns(false);
        _saldoDiarioRepositorio.ObterPorDataAsync(comando.Data, Arg.Any<CancellationToken>())
            .Returns((SaldoDiario?)null);

        var resultado = await _handler.Handle(comando, CancellationToken.None);

        resultado.Should().BeTrue();
    }
}
