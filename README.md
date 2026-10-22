# DesignPatterns

Practical implementations of classic design patterns in C# / .NET 9. Each pattern is a small, self-contained console project built around a realistic use case rather than abstract `Foo`/`Bar` examples.

## Patterns

| Pattern | Category | Use case | Project |
|---------|----------|----------|---------|
| **Singleton** | Creational | Thread-safe in-memory cache shared across the whole application | [`src/SingletonPattern`](src/SingletonPattern) |
| **Factory Method** | Creational | Notification dispatcher for SMS, Email, WhatsApp and Telegram | [`src/FactoryPattern`](src/FactoryPattern) |

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

## Running

Requires the [.NET 9 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/Martik2002/DesignPatterns.git
cd DesignPatterns

dotnet run --project src/SingletonPattern
dotnet run --project src/FactoryPattern
```

## License

[MIT](LICENSE)
