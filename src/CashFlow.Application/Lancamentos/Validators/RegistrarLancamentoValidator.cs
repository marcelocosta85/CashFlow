using CashFlow.Application.Lancamentos.Commands;
using FluentValidation;

namespace CashFlow.Application.Lancamentos.Validators;

public class RegistrarLancamentoValidator : AbstractValidator<RegistrarLancamentoCommand>
{
    public RegistrarLancamentoValidator()
    {
        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithMessage("O valor do lançamento deve ser maior que zero.");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de lançamento inválido.");

        RuleFor(x => x.Data)
            .Must(data => data.Date <= DateTime.UtcNow.Date)
            .WithMessage("Data não pode ser futura.");
    }
}
