using CashFlow.Application.Abstractions;
using CashFlow.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Consolidation.Worker.Persistence.Repositorios;

public class SaldoDiarioRepositorio : ISaldoDiarioRepositorio
{
    private readonly SaldoDiarioDbContext _dbContext;

    public SaldoDiarioRepositorio(SaldoDiarioDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SaldoDiario?> ObterPorDataAsync(DateTime data, CancellationToken cancellationToken)
    {
        return _dbContext.SaldosDiarios.FirstOrDefaultAsync(s => s.Data == data.Date, cancellationToken);
    }

    public Task<bool> LancamentoJaProcessadoAsync(Guid lancamentoId, CancellationToken cancellationToken)
    {
        return _dbContext.LancamentosProcessados.AnyAsync(l => l.LancamentoId == lancamentoId, cancellationToken);
    }

    public async Task RegistrarConsolidacaoAsync(SaldoDiario saldoDiario, Guid lancamentoId, CancellationToken cancellationToken)
    {
        // A restrição de chave primária em lancamentos_processados.LancamentoId é a garantia real de
        // atomicidade contra processamento concorrente da mesma mensagem: SaveChangesAsync já roda em
        // uma transação implícita do EF Core, e uma segunda inserção concorrente para o mesmo Id falha
        // aqui (violação de PK), não na checagem de idempotência feita antes pelo handler.
        if (_dbContext.Entry(saldoDiario).State == EntityState.Detached)
            _dbContext.SaldosDiarios.Add(saldoDiario);

        _dbContext.LancamentosProcessados.Add(new LancamentoProcessado(lancamentoId));

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
