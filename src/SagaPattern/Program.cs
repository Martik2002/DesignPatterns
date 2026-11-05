using SagaPattern.Models;
using SagaPattern.Saga;
using SagaPattern.Services;
using SagaPattern.Steps;

Console.WriteLine("=== Order Saga Pattern Demo ===\n");

// Scenario 1: every step commits, nothing is compensated.
Console.WriteLine("--- Scenario 1: happy path ---");
var inventory = new InventoryService(new Dictionary<string, int> { ["LAPTOP-X1"] = 10 });
var saga = BuildSaga(inventory, new PaymentService(), new ShippingService());

var order = new OrderContext("ORD-1001", "LAPTOP-X1", 2, 2400m, "customer@example.com");
PrintResult(await saga.ExecuteAsync(order), inventory, order.ProductSku);

// Scenario 2: the payment is declined, so the reserved stock has to go back.
Console.WriteLine("\n--- Scenario 2: payment declined ---");
inventory = new InventoryService(new Dictionary<string, int> { ["LAPTOP-X1"] = 10 });
saga = BuildSaga(inventory, new PaymentService(declineAbove: 2000m), new ShippingService());

order = new OrderContext("ORD-1002", "LAPTOP-X1", 2, 2400m, "customer@example.com");
PrintResult(await saga.ExecuteAsync(order), inventory, order.ProductSku);

// Scenario 3: the carrier is down after the money was already taken.
// Both earlier steps are undone, in reverse order: refund first, then release the stock.
Console.WriteLine("\n--- Scenario 3: carrier outage after payment ---");
inventory = new InventoryService(new Dictionary<string, int> { ["LAPTOP-X1"] = 10 });
saga = BuildSaga(inventory, new PaymentService(), new ShippingService(simulateCarrierOutage: true));

order = new OrderContext("ORD-1003", "LAPTOP-X1", 3, 3600m, "customer@example.com");
PrintResult(await saga.ExecuteAsync(order), inventory, order.ProductSku);

Console.WriteLine("\n=== Demo Completed Successfully ===");

static SagaOrchestrator<OrderContext> BuildSaga(
    InventoryService inventory,
    PaymentService payment,
    ShippingService shipping) =>
    new SagaOrchestrator<OrderContext>("PlaceOrder")
        .AddStep(new ReserveInventoryStep(inventory))
        .AddStep(new ChargePaymentStep(payment))
        .AddStep(new CreateShipmentStep(shipping))
        .AddStep(new SendConfirmationStep(new NotificationService()));

static void PrintResult(SagaResult result, InventoryService inventory, string productSku)
{
    if (result.IsSuccess)
    {
        Console.WriteLine($"   Result: order placed ({string.Join(" -> ", result.CompletedSteps)})");
    }
    else
    {
        Console.WriteLine($"   Result: order rejected at '{result.FailedStep}' — {result.Error?.Message}");
        Console.WriteLine($"   Rolled back: {(result.CompensatedSteps.Count == 0 ? "nothing" : string.Join(" -> ", result.CompensatedSteps))}");

        if (result.RequiresManualIntervention)
        {
            Console.WriteLine($"   Manual intervention needed for: {string.Join(", ", result.CompensationFailures.Select(f => f.StepName))}");
        }
    }

    Console.WriteLine($"   Stock of {productSku}: {inventory.GetAvailable(productSku)} unit(s)");
}
