using CashFlow.Domain.Exceptions;
using CashFlow.Domain.ValueObjects;
using FluentAssertions;

namespace CashFlow.Domain.Tests.ValueObjects;

public class ValorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Criar_DeveLancarExcecao_QuandoQuantiaForZeroOuNegativa(decimal quantia)
    {
        var acao = () => Valor.Criar(quantia);

        acao.Should().Throw<LancamentoInvalidoException>()
            .WithMessage("O valor do lançamento deve ser maior que zero.");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(1000.75)]
    public void Criar_DeveRetornarValor_QuandoQuantiaForPositiva(decimal quantia)
    {
        var valor = Valor.Criar(quantia);

        valor.Quantia.Should().Be(quantia);
    }
}
