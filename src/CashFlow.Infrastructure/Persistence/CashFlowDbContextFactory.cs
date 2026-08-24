using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CashFlow.Infrastructure.Persistence;

public class CashFlowDbContextFactory : IDesignTimeDbContextFactory<CashFlowDbContext>
{
    public CashFlowDbContext CreateDbContext(string[] args)
    {
        // Sem host/DI em design-time: o comando `dotnet ef` deve ser executado a partir do
        // diretorio do projeto de startup (CashFlow.Launches.Api), que e onde o appsettings.json
        // com a connection string "CashFlowDatabase" esta localizado.
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CashFlowDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("CashFlowDatabase"));

        return new CashFlowDbContext(optionsBuilder.Options);
    }
}
