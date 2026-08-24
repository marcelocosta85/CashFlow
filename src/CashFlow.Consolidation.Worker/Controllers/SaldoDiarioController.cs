using CashFlow.Application.Consolidacao.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace CashFlow.Consolidation.Worker.Controllers;

[ApiController]
[Route("saldos")]
public class SaldoDiarioController : ControllerBase
{
    private readonly ISender _sender;

    public SaldoDiarioController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{data:datetime}")]
    [ProducesResponseType(typeof(SaldoDiarioResultado), StatusCodes.Status200OK)]
    public async Task<ActionResult<SaldoDiarioResultado>> ObterPorData(DateTime data, CancellationToken cancellationToken)
    {
        var resultado = await _sender.Send(new ObterSaldoDiarioQuery(data), cancellationToken);

        return Ok(resultado);
    }
}
