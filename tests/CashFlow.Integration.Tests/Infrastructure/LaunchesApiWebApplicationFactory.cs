using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace CashFlow.Integration.Tests.Infrastructure;

public class LaunchesApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly string _rabbitMqHostName;
    private readonly int _rabbitMqPort;

    public LaunchesApiWebApplicationFactory(string connectionString, string rabbitMqHostName, int rabbitMqPort)
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
