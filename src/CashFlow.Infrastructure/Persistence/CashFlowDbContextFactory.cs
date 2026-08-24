using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CashFlow.Infrastructure.Persistence;

public class CashFlowDbContextFactory : IDesignTimeDbContextFactory<CashFlowDbContext>
{
    public CashFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CashFlowDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=cashflow;Username=cashflow;Password=cashflow");

        return new CashFlowDbContext(optionsBuilder.Options);
    }
}
