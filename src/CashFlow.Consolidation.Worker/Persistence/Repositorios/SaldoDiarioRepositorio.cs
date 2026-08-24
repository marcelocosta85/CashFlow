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

    public async Task AplicarLancamentoAsync(TipoLancamento tipo, decimal valor, DateTime data, CancellationToken cancellationToken)
    {
        var saldoDiario = await ObterPorDataAsync(data, cancellationToken);

        if (saldoDiario is null)
        {
            saldoDiario = new SaldoDiario(data);
            _dbContext.SaldosDiarios.Add(saldoDiario);
        }

        saldoDiario.AplicarLancamento(tipo, valor);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
