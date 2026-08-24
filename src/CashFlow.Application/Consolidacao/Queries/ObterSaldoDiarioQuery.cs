using Mediator;

namespace CashFlow.Application.Consolidacao.Queries;

public record ObterSaldoDiarioQuery(DateTime Data) : IQuery<SaldoDiarioResultado>;

public record SaldoDiarioResultado(
    DateTime Data,
    decimal TotalCreditos,
    decimal TotalDebitos,
    decimal SaldoConsolidado);
