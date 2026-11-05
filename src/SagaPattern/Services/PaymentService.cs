using SagaPattern.Interfaces;

namespace SagaPattern.Services;

/// <summary>
/// In-memory stand-in for a payment gateway. Declines anything above
/// <paramref name="declineAbove"/> so the demo can trigger a mid-saga failure.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly decimal _declineAbove;

    public PaymentService(decimal declineAbove = decimal.MaxValue)
    {
        _declineAbove = declineAbove;
    }

    public async Task<string> ChargeAsync(string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        if (amount > _declineAbove)
        {
            throw new InvalidOperationException($"Card declined: {amount:C} exceeds the limit of {_declineAbove:C}.");
        }

        var paymentId = $"PAY-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";
        Console.WriteLine($"   [Payment] Charged {amount:C} for {orderId} -> {paymentId}");
        return paymentId;
    }

    public async Task RefundAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);
        Console.WriteLine($"   [Payment] Refunded {paymentId}");
    }
}
