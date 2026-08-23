using CashFlow.Domain.Exceptions;

namespace CashFlow.Domain.ValueObjects;

public sealed class Valor : IEquatable<Valor>
{
    public decimal Quantia { get; }

    private Valor(decimal quantia)
    {
        Quantia = quantia;
    }

    public static Valor Criar(decimal quantia)
    {
        if (quantia <= 0)
            throw new LancamentoInvalidoException("O valor do lançamento deve ser maior que zero.");

        return new Valor(quantia);
    }

    public bool Equals(Valor? other)
    {
        if (other is null) return false;
        return Quantia == other.Quantia;
    }

    public override bool Equals(object? obj) => Equals(obj as Valor);

    public override int GetHashCode() => Quantia.GetHashCode();

    public override string ToString() => Quantia.ToString("F2");
}
