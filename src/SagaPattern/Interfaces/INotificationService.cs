namespace SagaPattern.Interfaces;

public interface INotificationService
{
    Task SendOrderConfirmationAsync(string email, string orderId, CancellationToken cancellationToken = default);
}
