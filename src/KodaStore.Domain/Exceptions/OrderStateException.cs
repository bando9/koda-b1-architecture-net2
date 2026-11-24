namespace KodaStore.Domain.Exceptions;

public sealed class OrderStateException : InvalidOperationException
{
    public OrderStateException(string message) : base(message)
    {

    }
}