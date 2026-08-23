using CashFlow.Application.Lancamentos.Commands;
using CashFlow.Application.Lancamentos.Validators;
using CashFlow.Domain.Enums;
using FluentAssertions;

namespace CashFlow.Application.Tests.Lancamentos;

public class RegistrarLancamentoValidatorTests
{
    private readonly RegistrarLancamentoValidator _validator = new();

    [Fact]
    public void Validate_DeveSerValido_QuandoComandoForCorreto()
    {
        var comando = new RegistrarLancamentoCommand(TipoLancamento.Credito, 100m, DateTime.UtcNow.Date, "Venda");

        var resultado = _validator.Validate(comando);

        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validate_DeveSerInvalido_QuandoValorForZeroOuNegativo(decimal valor)
    {
        var comando = new RegistrarLancamentoCommand(TipoLancamento.Credito, valor, DateTime.UtcNow.Date, "Venda");

        var resultado = _validator.Validate(comando);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(RegistrarLancamentoCommand.Valor));
    }

    [Fact]
    public void Validate_DeveSerInvalido_QuandoTipoForForaDoEnum()
    {
        var comando = new RegistrarLancamentoCommand((TipoLancamento)99, 100m, DateTime.UtcNow.Date, "Venda");

        var resultado = _validator.Validate(comando);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(RegistrarLancamentoCommand.Tipo));
    }

    [Fact]
    public void Validate_DeveSerInvalido_QuandoDataForFutura()
    {
        var comando = new RegistrarLancamentoCommand(
            TipoLancamento.Credito, 100m, DateTime.UtcNow.Date.AddDays(1), "Venda");

        var resultado = _validator.Validate(comando);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(RegistrarLancamentoCommand.Data));
    }
}
