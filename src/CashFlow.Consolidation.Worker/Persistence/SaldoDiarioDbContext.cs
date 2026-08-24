using CashFlow.Consolidation.Worker.Persistence.Configurations;
using CashFlow.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Consolidation.Worker.Persistence;

public class SaldoDiarioDbContext : DbContext
{
    public SaldoDiarioDbContext(DbContextOptions<SaldoDiarioDbContext> options) : base(options)
    {
    }

    public DbSet<SaldoDiario> SaldosDiarios => Set<SaldoDiario>();

    public DbSet<LancamentoProcessado> LancamentosProcessados => Set<LancamentoProcessado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("consolidation");
        modelBuilder.ApplyConfiguration(new SaldoDiarioConfiguration());
        modelBuilder.ApplyConfiguration(new LancamentoProcessadoConfiguration());
    }
}
