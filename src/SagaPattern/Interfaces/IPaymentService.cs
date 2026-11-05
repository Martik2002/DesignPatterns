namespace SagaPattern.Interfaces;

public interface IPaymentService
{
    /// <summary>Charges the customer and returns the payment id.</summary>
    Task<string> ChargeAsync(string orderId, decimal amount, CancellationToken cancellationToken = default);

    Task RefundAsync(string paymentId, CancellationToken cancellationToken = default);
}
