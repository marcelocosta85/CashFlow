using CashFlow.Domain.Entidades;
using CashFlow.Domain.Enums;

namespace CashFlow.Application.Abstractions;

public interface ISaldoDiarioRepositorio
{
    Task<SaldoDiario?> ObterPorDataAsync(DateTime data, CancellationToken cancellationToken);

    /// <returns>
    /// <see langword="true"/> se o lançamento foi aplicado ao saldo; <see langword="false"/> se já havia
    /// sido processado anteriormente (mensagem duplicada) e foi ignorado.
    /// </returns>
    Task<bool> AplicarLancamentoAsync(Guid lancamentoId, TipoLancamento tipo, decimal valor, DateTime data, CancellationToken cancellationToken);
}
