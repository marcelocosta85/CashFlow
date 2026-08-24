using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CashFlow.Consolidation.Worker.Persistence;

public class SaldoDiarioDbContextFactory : IDesignTimeDbContextFactory<SaldoDiarioDbContext>
{
    public SaldoDiarioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SaldoDiarioDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=cashflow;Username=cashflow;Password=cashflow");

        return new SaldoDiarioDbContext(optionsBuilder.Options);
    }
}
