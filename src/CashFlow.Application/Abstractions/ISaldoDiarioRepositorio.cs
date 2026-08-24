using CashFlow.Domain.Entidades;

namespace CashFlow.Application.Abstractions;

public interface ISaldoDiarioRepositorio
{
    Task<SaldoDiario?> ObterPorDataAsync(DateTime data, CancellationToken cancellationToken);

    Task<bool> LancamentoJaProcessadoAsync(Guid lancamentoId, CancellationToken cancellationToken);

    Task RegistrarConsolidacaoAsync(SaldoDiario saldoDiario, Guid lancamentoId, CancellationToken cancellationToken);
}
