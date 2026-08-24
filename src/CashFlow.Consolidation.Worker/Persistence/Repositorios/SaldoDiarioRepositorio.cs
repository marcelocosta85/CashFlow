using CashFlow.Application.Abstractions;
using CashFlow.Domain.Entidades;
using CashFlow.Domain.Enums;
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

    public async Task<bool> AplicarLancamentoAsync(Guid lancamentoId, TipoLancamento tipo, decimal valor, DateTime data, CancellationToken cancellationToken)
    {
        await using var transacao = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var jaProcessado = await _dbContext.LancamentosProcessados
            .AnyAsync(l => l.LancamentoId == lancamentoId, cancellationToken);

        if (jaProcessado)
            return false;

        var saldoDiario = await ObterPorDataAsync(data, cancellationToken);

        if (saldoDiario is null)
        {
            saldoDiario = new SaldoDiario(data);
            _dbContext.SaldosDiarios.Add(saldoDiario);
        }

        saldoDiario.AplicarLancamento(tipo, valor);
        _dbContext.LancamentosProcessados.Add(new LancamentoProcessado(lancamentoId));

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transacao.CommitAsync(cancellationToken);

        return true;
    }
}
