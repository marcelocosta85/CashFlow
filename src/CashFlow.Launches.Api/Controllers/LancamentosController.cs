using CashFlow.Application.Lancamentos.Commands;
using CashFlow.Launches.Api.Contracts;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Launches.Api.Controllers;

[ApiController]
[Route("lancamentos")]
public class LancamentosController : ControllerBase
{
    private readonly ISender _sender;

    public LancamentosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LancamentoCriadoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarLancamentoRequest request, CancellationToken cancellationToken)
    {
        var command = new RegistrarLancamentoCommand(request.Tipo, request.Valor, request.Data, request.Descricao);

        var id = await _sender.Send(command, cancellationToken);

        var response = new LancamentoCriadoResponse(id);

        return Created($"/lancamentos/{id}", response);
    }
}
