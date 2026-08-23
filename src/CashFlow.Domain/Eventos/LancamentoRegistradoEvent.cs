using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Eventos;

public record LancamentoRegistradoEvent(
    Guid LancamentoId,
    TipoLancamento Tipo,
    decimal Valor,
    DateTime Data,
    string Descricao);
