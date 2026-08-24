using FluentValidation;
using FluentValidation.Results;
using Mediator;

namespace CashFlow.Application.Common.Behaviors;

public class ValidationBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly IEnumerable<IValidator<TMessage>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TMessage>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TMessage>(message);
            var falhas = new List<ValidationFailure>();

            foreach (var validator in _validators)
            {
                var resultado = await validator.ValidateAsync(context, cancellationToken);
                falhas.AddRange(resultado.Errors);
            }

            if (falhas.Count > 0)
                throw new ValidationException(falhas);
        }

        return await next(message, cancellationToken);
    }
}
