using CashFlow.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Launches.Api.ErrorHandling;

public class LancamentoInvalidoExceptionHandler : IExceptionHandler
{
    private readonly ILogger<LancamentoInvalidoExceptionHandler> _logger;

    public LancamentoInvalidoExceptionHandler(ILogger<LancamentoInvalidoExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var mensagens = exception switch
        {
            LancamentoInvalidoException ex => [ex.Message],
            ValidationException ex => ex.Errors.Select(e => e.ErrorMessage).ToArray(),
            _ => (string[]?)null
        };

        if (mensagens is null)
            return false;

        _logger.LogWarning(exception, "Requisição inválida: {Mensagens}", string.Join(" | ", mensagens));

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Requisição inválida",
            Detail = string.Join(" ", mensagens)
        }, cancellationToken);

        return true;
    }
}
