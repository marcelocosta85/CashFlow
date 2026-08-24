using CashFlow.Domain.Enums;

namespace CashFlow.Launches.Api.Contracts;

public record RegistrarLancamentoRequest(
    TipoLancamento Tipo,
    decimal Valor,
    DateTime Data,
    string Descricao);
