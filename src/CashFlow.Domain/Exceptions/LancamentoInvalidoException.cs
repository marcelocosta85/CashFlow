namespace CashFlow.Domain.Exceptions;

public class LancamentoInvalidoException : Exception
{
    public LancamentoInvalidoException(string mensagem) : base(mensagem)
    {
    }
}
