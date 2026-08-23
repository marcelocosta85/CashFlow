using CashFlow.Domain.Entidades;

namespace CashFlow.Application.Abstractions;

public interface ILancamentoRepositorio
{
    Task AdicionarAsync(Lancamento lancamento, CancellationToken cancellationToken);
}
