using CashFlow.Application.Abstractions;
using CashFlow.Application.Lancamentos.Commands;
using CashFlow.Application.Lancamentos.Handlers;
using CashFlow.Domain.Entidades;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Eventos;
using FluentAssertions;
using NSubstitute;

namespace CashFlow.Application.Tests.Lancamentos;

public class RegistrarLancamentoHandlerTests
{
    private readonly ILancamentoRepositorio _lancamentoRepositorio = Substitute.For<ILancamentoRepositorio>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly RegistrarLancamentoHandler _handler;

    public RegistrarLancamentoHandlerTests()
    {
        _handler = new RegistrarLancamentoHandler(_lancamentoRepositorio, _eventPublisher);
    }

    [Fact]
    public async Task Handle_DevePersistirLancamento_QuandoComandoForValido()
    {
        var comando = new RegistrarLancamentoCommand(TipoLancamento.Credito, 100m, DateTime.UtcNow.Date, "Venda");

        await _handler.Handle(comando, CancellationToken.None);

        await _lancamentoRepositorio.Received(1).AdicionarAsync(
            Arg.Is<Lancamento>(l => l.Tipo == TipoLancamento.Credito && l.Valor.Quantia == 100m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DevePublicarEvento_QuandoComandoForValido()
    {
        var comando = new RegistrarLancamentoCommand(TipoLancamento.Debito, 50m, DateTime.UtcNow.Date, "Compra");

        await _handler.Handle(comando, CancellationToken.None);

        await _eventPublisher.Received(1).PublicarAsync(
            Arg.Is<LancamentoRegistradoEvent>(e => e.Tipo == TipoLancamento.Debito && e.Valor == 50m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveRetornarIdDoLancamentoCriado()
    {
        var comando = new RegistrarLancamentoCommand(TipoLancamento.Credito, 100m, DateTime.UtcNow.Date, "Venda");

        var id = await _handler.Handle(comando, CancellationToken.None);

        id.Should().NotBeEmpty();
    }
}
