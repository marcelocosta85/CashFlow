using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace CashFlow.Integration.Tests.Infrastructure;

/// <summary>
/// Sobe um Postgres e um RabbitMQ reais (Testcontainers) e hospeda os dois serviços
/// (Launches.Api e Consolidation.Worker) apontando para essa mesma infraestrutura,
/// reproduzindo o fluxo ponta a ponta descrito em RF01/RF02/RF03.
/// </summary>
public class CashFlowIntegrationTestFixture : IAsyncLifetime
{
    private const int RabbitMqPortoPadrao = 5672;

    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("cashflow")
        .WithUsername("cashflow")
        .WithPassword("cashflow")
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder("rabbitmq:3-management-alpine")
        .WithUsername("cashflow")
        .WithPassword("cashflow")
        .Build();

    public LaunchesApiWebApplicationFactory LaunchesApiFactory { get; private set; } = null!;

    public ConsolidationWorkerWebApplicationFactory ConsolidationWorkerFactory { get; private set; } = null!;

    public string RabbitMqHostName { get; private set; } = null!;

    public int RabbitMqPort { get; private set; }

    public string RabbitMqUserName => "cashflow";

    public string RabbitMqPassword => "cashflow";

    public async ValueTask InitializeAsync()
    {
        await Task.WhenAll(_postgresContainer.StartAsync(), _rabbitMqContainer.StartAsync());

        RabbitMqHostName = _rabbitMqContainer.Hostname;
        RabbitMqPort = _rabbitMqContainer.GetMappedPublicPort(RabbitMqPortoPadrao);

        var connectionString = _postgresContainer.GetConnectionString();

        LaunchesApiFactory = new LaunchesApiWebApplicationFactory(connectionString, RabbitMqHostName, RabbitMqPort);
        ConsolidationWorkerFactory = new ConsolidationWorkerWebApplicationFactory(connectionString, RabbitMqHostName, RabbitMqPort);

        // Acessar Server força a construção (e o Start) de cada host — aplica as migrations
        // no OnStartup e inicia o RabbitMqConsumer (HostedService) do Worker.
        _ = LaunchesApiFactory.Server;
        _ = ConsolidationWorkerFactory.Server;
    }

    public async ValueTask DisposeAsync()
    {
        await LaunchesApiFactory.DisposeAsync();
        await ConsolidationWorkerFactory.DisposeAsync();
        await _postgresContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
    }
}
