using CashFlow.Application.Abstractions;
using CashFlow.Application.Consolidacao.Commands;
using CashFlow.Domain.Entidades;
using Mediator;

namespace CashFlow.Application.Consolidacao.Handlers;

public class ConsolidarLancamentoHandler : IRequestHandler<ConsolidarLancamentoCommand, bool>
{
    private readonly ISaldoDiarioRepositorio _saldoDiarioRepositorio;

    public ConsolidarLancamentoHandler(ISaldoDiarioRepositorio saldoDiarioRepositorio)
    {
        _saldoDiarioRepositorio = saldoDiarioRepositorio;
    }

    public async ValueTask<bool> Handle(ConsolidarLancamentoCommand request, CancellationToken cancellationToken)
    {
        var jaProcessado = await _saldoDiarioRepositorio.LancamentoJaProcessadoAsync(request.LancamentoId, cancellationToken);

        if (jaProcessado)
            return false;

        var saldoDiario = await _saldoDiarioRepositorio.ObterPorDataAsync(request.Data, cancellationToken)
            ?? new SaldoDiario(request.Data);

        saldoDiario.AplicarLancamento(request.Tipo, request.Valor);

        await _saldoDiarioRepositorio.RegistrarConsolidacaoAsync(saldoDiario, request.LancamentoId, cancellationToken);

        return true;
    }
}
