extern alias WorkerHost;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using WorkerProgram = WorkerHost::Program;

namespace CashFlow.Integration.Tests.Infrastructure;

public class ConsolidationWorkerWebApplicationFactory : WebApplicationFactory<WorkerProgram>
{
    private readonly string _connectionString;
    private readonly string _rabbitMqHostName;
    private readonly int _rabbitMqPort;

    public ConsolidationWorkerWebApplicationFactory(string connectionString, string rabbitMqHostName, int rabbitMqPort)
    {
        _connectionString = connectionString;
        _rabbitMqHostName = rabbitMqHostName;
        _rabbitMqPort = rabbitMqPort;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:CashFlowDatabase"] = _connectionString,
                ["RabbitMq:HostName"] = _rabbitMqHostName,
                ["RabbitMq:Port"] = _rabbitMqPort.ToString(),
                ["RabbitMq:UserName"] = "cashflow",
                ["RabbitMq:Password"] = "cashflow"
            });
        });
    }
}
