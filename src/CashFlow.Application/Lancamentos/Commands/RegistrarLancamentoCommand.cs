using CashFlow.Domain.Enums;
using Mediator;

namespace CashFlow.Application.Lancamentos.Commands;

public record RegistrarLancamentoCommand(
    TipoLancamento Tipo,
    decimal Valor,
    DateTime Data,
    string Descricao) : IRequest<Guid>;
