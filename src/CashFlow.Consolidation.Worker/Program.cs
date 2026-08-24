using CashFlow.Application.Abstractions;
using CashFlow.Consolidation.Worker.Messaging;
using CashFlow.Consolidation.Worker.Persistence;
using CashFlow.Consolidation.Worker.Persistence.Repositorios;
using CashFlow.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Mediator registra handlers dos dois bounded contexts (visíveis via CashFlow.Application) neste host;
// os do outro serviço nunca são chamados e não têm repositório registrado, então ValidateOnBuild fica off.
builder.Host.UseDefaultServiceProvider((_, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = false;
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SaldoDiarioDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CashFlowDatabase")));

builder.Services.AddScoped<ISaldoDiarioRepositorio, SaldoDiarioRepositorio>();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddSingleton<RabbitMqConnectionManager>();
builder.Services.AddHostedService<RabbitMqConsumer>();

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SaldoDiarioDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
