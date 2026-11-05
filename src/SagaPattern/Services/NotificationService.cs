using SagaPattern.Interfaces;

namespace SagaPattern.Services;

public class NotificationService : INotificationService
{
    public async Task SendOrderConfirmationAsync(string email, string orderId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);
        Console.WriteLine($"   [Notification] Confirmation for {orderId} sent to {email}");
    }
}
