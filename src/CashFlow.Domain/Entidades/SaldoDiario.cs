using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Entidades;

public class SaldoDiario
{
    public DateTime Data { get; private set; }
    public decimal TotalCreditos { get; private set; }
    public decimal TotalDebitos { get; private set; }
    public decimal SaldoConsolidado => TotalCreditos - TotalDebitos;

    private SaldoDiario()
    {
    }

    public SaldoDiario(DateTime data)
    {
        Data = data.Date;
    }

    public void AplicarLancamento(TipoLancamento tipo, decimal valor)
    {
        if (tipo == TipoLancamento.Credito)
            TotalCreditos += valor;
        else
            TotalDebitos += valor;
    }
}
