namespace CashFlow.Consolidation.Worker.Persistence;

public class LancamentoProcessado
{
    public Guid LancamentoId { get; private set; }

    public DateTime ProcessadoEm { get; private set; }

    private LancamentoProcessado()
    {
    }

    public LancamentoProcessado(Guid lancamentoId)
    {
        LancamentoId = lancamentoId;
        ProcessadoEm = DateTime.UtcNow;
    }
}
