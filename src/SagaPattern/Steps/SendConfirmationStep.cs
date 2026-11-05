using SagaPattern.Interfaces;
using SagaPattern.Models;

namespace SagaPattern.Steps;

/// <summary>
/// The last step of the saga. An email cannot be un-sent, so its compensation does nothing.
/// Steps like this belong at the end of the pipeline precisely because they are irreversible.
/// </summary>
public class SendConfirmationStep : ISagaStep<OrderContext>
{
    private readonly INotificationService _notificationService;

    public SendConfirmationStep(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public string Name => "Send confirmation";

    public Task ExecuteAsync(OrderContext context, CancellationToken cancellationToken = default) =>
        _notificationService.SendOrderConfirmationAsync(context.CustomerEmail, context.OrderId, cancellationToken);

    public Task CompensateAsync(OrderContext context, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
