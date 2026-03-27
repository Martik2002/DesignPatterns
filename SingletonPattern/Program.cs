using SingletonPattern.Pattern;

InMemoryCache.Instance.Set("User_1", "Test User Data");

var cachedUser = InMemoryCache.Instance.Get("User_1");
Console.WriteLine($"Retrieved User: {cachedUser}");

InMemoryCache.Instance.Clear();
Console.WriteLine($"Clearing Cache");

var clearedUser = InMemoryCache.Instance.Get("User_1");
Console.WriteLine($"After Clearing Cache, Retrieved User: {clearedUser}");

var anotherReference = InMemoryCache.Instance;

if (ReferenceEquals(InMemoryCache.Instance, anotherReference))
{
    Console.WriteLine("Success: It is the same instance. Data is shared!");
}