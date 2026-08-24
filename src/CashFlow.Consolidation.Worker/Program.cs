using CashFlow.Application.Abstractions;
using CashFlow.Consolidation.Worker.Persistence;
using CashFlow.Consolidation.Worker.Persistence.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SaldoDiarioDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CashFlowDatabase")));

builder.Services.AddScoped<ISaldoDiarioRepositorio, SaldoDiarioRepositorio>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SaldoDiarioDbContext>();
    dbContext.Database.Migrate();
}

host.Run();
