namespace CashFlow.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string UserName { get; set; } = "cashflow";

    public string Password { get; set; } = "cashflow";
}
