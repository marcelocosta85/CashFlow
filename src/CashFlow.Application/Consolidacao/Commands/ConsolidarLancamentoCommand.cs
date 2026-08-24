using CashFlow.Domain.Enums;
using Mediator;

namespace CashFlow.Application.Consolidacao.Commands;

public record ConsolidarLancamentoCommand(
    Guid LancamentoId,
    TipoLancamento Tipo,
    decimal Valor,
    DateTime Data) : IRequest<bool>;
