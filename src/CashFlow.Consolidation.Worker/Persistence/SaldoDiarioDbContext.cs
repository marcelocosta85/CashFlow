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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("consolidation");
        modelBuilder.ApplyConfiguration(new SaldoDiarioConfiguration());
    }
}
