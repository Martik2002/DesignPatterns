namespace SagaPattern.Interfaces;

public interface IInventoryService
{
    /// <summary>Reserves stock and returns the reservation id.</summary>
    Task<string> ReserveAsync(string productSku, int quantity, CancellationToken cancellationToken = default);

    Task ReleaseAsync(string reservationId, CancellationToken cancellationToken = default);
}
