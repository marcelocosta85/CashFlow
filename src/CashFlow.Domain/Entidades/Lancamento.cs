using CashFlow.Domain.Enums;
using CashFlow.Domain.Exceptions;
using CashFlow.Domain.ValueObjects;

namespace CashFlow.Domain.Entidades;

public class Lancamento
{
    public Guid Id { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public Valor Valor { get; private set; } = null!;
    public DateTime Data { get; private set; }
    public string Descricao { get; private set; } = string.Empty;

    private Lancamento()
    {
    }

    public static Lancamento Registrar(TipoLancamento tipo, decimal valor, DateTime data, string descricao)
    {
        if (data.Date > DateTime.UtcNow.Date)
            throw new LancamentoInvalidoException("Data não pode ser futura.");

        return new Lancamento
        {
            Id = Guid.NewGuid(),
            Tipo = tipo,
            Valor = Valor.Criar(valor),
            Data = data,
            Descricao = descricao
        };
    }
}
