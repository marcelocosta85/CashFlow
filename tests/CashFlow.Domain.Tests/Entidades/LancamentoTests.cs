using CashFlow.Domain.Entidades;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Exceptions;
using FluentAssertions;

namespace CashFlow.Domain.Tests.Entidades;

public class LancamentoTests
{
    [Fact]
    public void Registrar_DeveCriarLancamento_QuandoDadosForemValidos()
    {
        var data = DateTime.UtcNow.Date;

        var lancamento = Lancamento.Registrar(TipoLancamento.Credito, 100m, data, "Venda de produto");

        lancamento.Id.Should().NotBeEmpty();
        lancamento.Tipo.Should().Be(TipoLancamento.Credito);
        lancamento.Valor.Quantia.Should().Be(100m);
        lancamento.Data.Should().Be(data);
        lancamento.Descricao.Should().Be("Venda de produto");
    }

    [Fact]
    public void Registrar_DeveLancarExcecao_QuandoDataForFutura()
    {
        var dataFutura = DateTime.UtcNow.Date.AddDays(1);

        var acao = () => Lancamento.Registrar(TipoLancamento.Debito, 50m, dataFutura, "Compra");

        acao.Should().Throw<LancamentoInvalidoException>()
            .WithMessage("Data não pode ser futura.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Registrar_DeveLancarExcecao_QuandoValorForZeroOuNegativo(decimal valor)
    {
        var acao = () => Lancamento.Registrar(TipoLancamento.Credito, valor, DateTime.UtcNow.Date, "Descrição");

        acao.Should().Throw<LancamentoInvalidoException>();
    }
}
