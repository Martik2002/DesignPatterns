using SagaPattern.Interfaces;
using SagaPattern.Models;

namespace SagaPattern.Steps;

public class ReserveInventoryStep : ISagaStep<OrderContext>
{
    private readonly IInventoryService _inventoryService;

    public ReserveInventoryStep(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public string Name => "Reserve inventory";

    public async Task ExecuteAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        context.ReservationId = await _inventoryService.ReserveAsync(context.ProductSku, context.Quantity, cancellationToken);
    }

    public async Task CompensateAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        if (context.ReservationId is null)
        {
            return;
        }

        await _inventoryService.ReleaseAsync(context.ReservationId, cancellationToken);
        context.ReservationId = null;
    }
}
