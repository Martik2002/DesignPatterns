namespace SagaPattern.Interfaces;

public interface IShippingService
{
    /// <summary>Books a shipment with the carrier and returns the shipment id.</summary>
    Task<string> CreateShipmentAsync(string orderId, string productSku, int quantity, CancellationToken cancellationToken = default);

    Task CancelShipmentAsync(string shipmentId, CancellationToken cancellationToken = default);
}
