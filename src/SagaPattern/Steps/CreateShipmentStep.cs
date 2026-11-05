using SagaPattern.Interfaces;
using SagaPattern.Models;

namespace SagaPattern.Steps;

public class CreateShipmentStep : ISagaStep<OrderContext>
{
    private readonly IShippingService _shippingService;

    public CreateShipmentStep(IShippingService shippingService)
    {
        _shippingService = shippingService;
    }

    public string Name => "Create shipment";

    public async Task ExecuteAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        context.ShipmentId = await _shippingService.CreateShipmentAsync(
            context.OrderId,
            context.ProductSku,
            context.Quantity,
            cancellationToken);
    }

    public async Task CompensateAsync(OrderContext context, CancellationToken cancellationToken = default)
    {
        if (context.ShipmentId is null)
        {
            return;
        }

        await _shippingService.CancelShipmentAsync(context.ShipmentId, cancellationToken);
        context.ShipmentId = null;
    }
}
