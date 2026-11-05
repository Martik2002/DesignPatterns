using System.Collections.Concurrent;
using SagaPattern.Interfaces;

namespace SagaPattern.Services;

/// <summary>
/// In-memory stand-in for a real inventory microservice. Keeps a stock counter so the
/// demo can show that a released reservation really does put the units back.
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly ConcurrentDictionary<string, int> _stock;
    private readonly ConcurrentDictionary<string, (string Sku, int Quantity)> _reservations = new();

    public InventoryService(IDictionary<string, int> initialStock)
    {
        _stock = new ConcurrentDictionary<string, int>(initialStock);
    }

    public int GetAvailable(string productSku) => _stock.TryGetValue(productSku, out var units) ? units : 0;

    public async Task<string> ReserveAsync(string productSku, int quantity, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        var available = GetAvailable(productSku);
        if (available < quantity)
        {
            throw new InvalidOperationException($"Only {available} unit(s) of {productSku} left, {quantity} requested.");
        }

        _stock[productSku] = available - quantity;

        var reservationId = $"RES-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";
        _reservations[reservationId] = (productSku, quantity);

        Console.WriteLine($"   [Inventory] Reserved {quantity} x {productSku} -> {reservationId} ({_stock[productSku]} left)");
        return reservationId;
    }

    public async Task ReleaseAsync(string reservationId, CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);

        if (!_reservations.TryRemove(reservationId, out var reservation))
        {
            // Compensation must be idempotent: an unknown reservation is already released.
            Console.WriteLine($"   [Inventory] Reservation {reservationId} already released, nothing to do");
            return;
        }

        _stock.AddOrUpdate(reservation.Sku, reservation.Quantity, (_, units) => units + reservation.Quantity);
        Console.WriteLine($"   [Inventory] Released {reservationId} ({_stock[reservation.Sku]} back in stock)");
    }
}
