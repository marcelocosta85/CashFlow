namespace CashFlow.Integration.Tests.Infrastructure;

/// <summary>
/// Polling simples para aguardar efeitos de processamento assíncrono (fila + worker)
/// nos testes ponta a ponta, evitando Thread.Sleep fixo e flakiness por timing.
/// </summary>
public static class Espera
{
    public static async Task AteAsync(Func<Task<bool>> condicao, int timeoutSegundos = 15, int intervaloMs = 250)
    {
        var limite = DateTime.UtcNow.AddSeconds(timeoutSegundos);

        while (DateTime.UtcNow < limite)
        {
            if (await condicao())
                return;

            await Task.Delay(intervaloMs);
        }

        throw new TimeoutException($"Condição não satisfeita dentro do tempo limite de {timeoutSegundos}s.");
    }
}
