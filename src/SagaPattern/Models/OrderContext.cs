namespace SagaPattern.Models;

/// <summary>
/// Carries the state of a single order through every step of the saga.
/// Each step reads what it needs and writes back the identifiers its own
/// compensation will require later (reservation id, payment id, shipment id).
/// </summary>
public class OrderContext
{
    public OrderContext(string orderId, string productSku, int quantity, decimal amount, string customerEmail)
    {
        OrderId = orderId;
        ProductSku = productSku;
        Quantity = quantity;
        Amount = amount;
        CustomerEmail = customerEmail;
    }

    public string OrderId { get; }

    public string ProductSku { get; }

    public int Quantity { get; }

    public decimal Amount { get; }

    public string CustomerEmail { get; }

    /// <summary>Set by the inventory step; required to release the stock again.</summary>
    public string? ReservationId { get; set; }

    /// <summary>Set by the payment step; required to issue a refund.</summary>
    public string? PaymentId { get; set; }

    /// <summary>Set by the shipping step; required to cancel the shipment.</summary>
    public string? ShipmentId { get; set; }
}
