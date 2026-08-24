using CashFlow.Application.Abstractions;
using CashFlow.Application.Common.Behaviors;
using CashFlow.Application.Lancamentos.Validators;
using CashFlow.Infrastructure.Messaging;
using CashFlow.Infrastructure.Persistence;
using CashFlow.Infrastructure.Persistence.Repositorios;
using CashFlow.Launches.Api.ErrorHandling;
using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Mediator registra handlers dos dois bounded contexts (visíveis via CashFlow.Application) neste host;
// os do outro serviço nunca são chamados e não têm repositório registrado, então ValidateOnBuild fica off.
builder.Host.UseDefaultServiceProvider((_, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = false;
});

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CashFlowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CashFlowDatabase")));

builder.Services.AddScoped<ILancamentoRepositorio, LancamentoRepositorio>();

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddSingleton<RabbitMqConnectionManager>();
builder.Services.AddSingleton<IEventPublisher, RabbitMqPublisher>();

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
builder.Services.AddValidatorsFromAssemblyContaining<RegistrarLancamentoValidator>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddExceptionHandler<LancamentoInvalidoExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CashFlowDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
