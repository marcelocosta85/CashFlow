using CashFlow.Application.Abstractions;
using CashFlow.Application.Consolidacao.Queries;
using Mediator;

namespace CashFlow.Application.Consolidacao.Handlers;

public class ObterSaldoDiarioHandler : IQueryHandler<ObterSaldoDiarioQuery, SaldoDiarioResultado>
{
    private readonly ISaldoDiarioRepositorio _saldoDiarioRepositorio;

    public ObterSaldoDiarioHandler(ISaldoDiarioRepositorio saldoDiarioRepositorio)
    {
        _saldoDiarioRepositorio = saldoDiarioRepositorio;
    }

    public async ValueTask<SaldoDiarioResultado> Handle(ObterSaldoDiarioQuery query, CancellationToken cancellationToken)
    {
        var saldoDiario = await _saldoDiarioRepositorio.ObterPorDataAsync(query.Data, cancellationToken);

        if (saldoDiario is null)
            return new SaldoDiarioResultado(query.Data.Date, 0m, 0m, 0m);

        return new SaldoDiarioResultado(
            saldoDiario.Data,
            saldoDiario.TotalCreditos,
            saldoDiario.TotalDebitos,
            saldoDiario.SaldoConsolidado);
    }
}
