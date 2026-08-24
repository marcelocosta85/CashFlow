using CashFlow.Application.Abstractions;
using CashFlow.Domain.Entidades;

namespace CashFlow.Infrastructure.Persistence.Repositorios;

public class LancamentoRepositorio : ILancamentoRepositorio
{
    private readonly CashFlowDbContext _dbContext;

    public LancamentoRepositorio(CashFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Lancamento lancamento, CancellationToken cancellationToken)
    {
        await _dbContext.Lancamentos.AddAsync(lancamento, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
