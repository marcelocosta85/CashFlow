namespace CashFlow.Application.Abstractions;

public interface IEventPublisher
{
    Task PublicarAsync<TEvento>(TEvento evento, CancellationToken cancellationToken) where TEvento : class;
}
