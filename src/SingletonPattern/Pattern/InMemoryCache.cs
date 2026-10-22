using System.Collections.Concurrent;

namespace SingletonPattern.Pattern;
/// <summary>
/// Implements the Singleton Design Pattern using a thread-safe, lazy-loaded approach.
/// This service acts as a centralized In-Memory Cache to ensure data consistency 
/// across the entire application lifecycle by maintaining a single shared state.
/// Lazy&lt;T&gt; makes instance creation thread-safe; ConcurrentDictionary makes reads and writes thread-safe.
/// </summary>
public sealed class InMemoryCache
{
    private static readonly Lazy<InMemoryCache> _instance = new(() => new InMemoryCache());

    private readonly ConcurrentDictionary<string, object> _storage = new();
    private InMemoryCache()
    {
        Console.WriteLine("[System] Cache Service initialized.");
    }
    public static InMemoryCache Instance => _instance.Value;

    public void Set(string key, object value)
    {
        _storage[key] = value;
        Console.WriteLine($"[Cache] Saved: {key}");
    }

    public object? Get(string key)
    {
        return _storage.TryGetValue(key, out var value) ? value : null;
    }

    public void Clear() => _storage.Clear();
}