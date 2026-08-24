using CashFlow.Domain.Entidades;
using CashFlow.Domain.Enums;

namespace CashFlow.Application.Abstractions;

public interface ISaldoDiarioRepositorio
{
    Task<SaldoDiario?> ObterPorDataAsync(DateTime data, CancellationToken cancellationToken);

    Task AplicarLancamentoAsync(TipoLancamento tipo, decimal valor, DateTime data, CancellationToken cancellationToken);
}
