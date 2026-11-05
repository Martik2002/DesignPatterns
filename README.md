# DesignPatterns

[![Build](https://github.com/Martik2002/DesignPatterns/actions/workflows/build.yml/badge.svg)](https://github.com/Martik2002/DesignPatterns/actions/workflows/build.yml)

Practical implementations of classic design patterns in C# / .NET 9. Each pattern is a small, self-contained console project built around a realistic use case rather than abstract `Foo`/`Bar` examples.

## Patterns

| Pattern | Category | Use case | Project |
|---------|----------|----------|---------|
| **Singleton** | Creational | Thread-safe in-memory cache shared across the whole application | [`src/SingletonPattern`](src/SingletonPattern) |
| **Factory Method** | Creational | Notification dispatcher for SMS, Email, WhatsApp and Telegram | [`src/FactoryPattern`](src/FactoryPattern) |
| **Saga** | Behavioral / Distributed | Order placement across inventory, payment and shipping, with compensating transactions | [`src/SagaPattern`](src/SagaPattern) |

More patterns are being added over time.

### Singleton — In-Memory Cache

`InMemoryCache` exposes a single shared instance through `Lazy<T>`, so the instance is created on first use and creation is thread-safe without explicit locks. The storage itself is a `ConcurrentDictionary`, so concurrent reads and writes are safe too. The class is `sealed` with a private constructor, so no other instance can be created.

```csharp
InMemoryCache.Instance.Set("User_1", "Test User Data");
var user = InMemoryCache.Instance.Get("User_1");
```

### Factory Method — Notification Dispatcher

`MessageFactory` creates the right `IMessageService` implementation for a given `MessageType`. Client code depends only on the `IMessageService` abstraction, never on concrete providers, which keeps it decoupled and easy to test.

```csharp
var sms = MessageFactory.CreateService(MessageType.Sms);
sms.Send("+1234567890", "Your verification code is 5542.");
```

### Saga — Order Placement

An order has to reserve stock, charge the card, book a shipment and send a confirmation. Those live in different services, so there is no shared transaction to roll back. `SagaOrchestrator<TContext>` runs each `ISagaStep` in order, remembers what committed, and compensates the completed steps in reverse order as soon as one of them fails — releasing the reservation, refunding the payment, cancelling the shipment.

```csharp
var result = await new SagaOrchestrator<OrderContext>("PlaceOrder")
    .AddStep(new ReserveInventoryStep(inventoryService))
    .AddStep(new ChargePaymentStep(paymentService))
    .AddStep(new CreateShipmentStep(shippingService))
    .AddStep(new SendConfirmationStep(notificationService))
    .ExecuteAsync(order);
```

## Running

Requires the [.NET 9 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/Martik2002/DesignPatterns.git
cd DesignPatterns

dotnet run --project src/SingletonPattern
dotnet run --project src/FactoryPattern
dotnet run --project src/SagaPattern
```

## License

[MIT](LICENSE)
