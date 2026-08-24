using CashFlow.Domain.Entidades;
using CashFlow.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Persistence;

public class CashFlowDbContext : DbContext
{
    public CashFlowDbContext(DbContextOptions<CashFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("launches");
        modelBuilder.ApplyConfiguration(new LancamentoConfiguration());
    }
}
