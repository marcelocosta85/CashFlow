using CashFlow.Application.Abstractions;
using CashFlow.Application.Lancamentos.Commands;
using CashFlow.Domain.Entidades;
using CashFlow.Domain.Eventos;
using Mediator;

namespace CashFlow.Application.Lancamentos.Handlers;

public class RegistrarLancamentoHandler : IRequestHandler<RegistrarLancamentoCommand, Guid>
{
    private readonly ILancamentoRepositorio _lancamentoRepositorio;
    private readonly IEventPublisher _eventPublisher;

    public RegistrarLancamentoHandler(ILancamentoRepositorio lancamentoRepositorio, IEventPublisher eventPublisher)
    {
        _lancamentoRepositorio = lancamentoRepositorio;
        _eventPublisher = eventPublisher;
    }

    public async ValueTask<Guid> Handle(RegistrarLancamentoCommand request, CancellationToken cancellationToken)
    {
        var lancamento = Lancamento.Registrar(request.Tipo, request.Valor, request.Data, request.Descricao);

        await _lancamentoRepositorio.AdicionarAsync(lancamento, cancellationToken);

        var evento = new LancamentoRegistradoEvent(
            lancamento.Id,
            lancamento.Tipo,
            lancamento.Valor.Quantia,
            lancamento.Data,
            lancamento.Descricao);

        await _eventPublisher.PublicarAsync(evento, cancellationToken);

        return lancamento.Id;
    }
}
