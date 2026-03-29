# Singleton Pattern: In-Memory Cache Example

This repository demonstrates the **Singleton Design Pattern** in .NET, implementing a thread-safe **In-Memory Cache**.

## Why Singleton for Caching?
In a software system, a Cache must be a "Single Source of Truth." If multiple instances of a cache existed, different parts of the application would see different data, leading to inconsistency and high memory usage.

## Implementation Highlights
* **Thread-Safety:** Implemented using `Lazy<T>`, which is the modern and most efficient way to handle concurrency in .NET without explicit `lock` statements.
* **Lazy Initialization:** The instance is only created when first accessed, saving resources during application startup.
* **Sealed Class:** Prevents inheritance to maintain the pattern's integrity.
