using SagaPattern.Interfaces;

namespace SagaPattern.Services;

/// <summary>
/// In-memory stand-in for a carrier API. Can be put into an outage so the demo can fail
/// at the last step, after both the inventory and the payment have already been committed.
/// </summary>
public class ShippingService : IShippingService
{
    private readonly bool _simulateCarrierOutage;

    public ShippingService(bool simulateCarrierOutage = false)
    {
        _simulateCarrierOutage = simulateCarrierOutage;
    }

    public async Task<string> CreateShipmentAsync(string orderId, string productSku, int quantity, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        if (_simulateCarrierOutage)
        {
            throw new HttpRequestException("Carrier API is unavailable (503).");
        }

        var shipmentId = $"SHP-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";
        Console.WriteLine($"   [Shipping] Booked {quantity} x {productSku} for {orderId} -> {shipmentId}");
        return shipmentId;
    }

    public async Task CancelShipmentAsync(string shipmentId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);
        Console.WriteLine($"   [Shipping] Cancelled {shipmentId}");
    }
}
