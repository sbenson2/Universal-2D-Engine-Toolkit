namespace MyGame.Core;

/// <summary>
/// Simple static service locator for shared services (GraphicsDevice, SpriteBatch, etc.).
/// Register services during initialization, resolve them anywhere.
/// </summary>
/// <remarks>
/// Use sparingly — this is a convenience pattern, not a dependency injection framework.
/// Good for engine-level services; prefer constructor parameters for game-specific dependencies.
/// </remarks>
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    /// <summary>
    /// Register a service instance by its type.
    /// </summary>
    /// <typeparam name="T">The service type (used as the lookup key).</typeparam>
    /// <param name="service">The service instance.</param>
    public static void Register<T>(T service) where T : notnull
    {
        _services[typeof(T)] = service;
    }

    /// <summary>
    /// Resolve a registered service by type.
    /// </summary>
    /// <typeparam name="T">The service type to resolve.</typeparam>
    /// <returns>The registered service instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
    public static T Get<T>() where T : notnull
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }

        throw new InvalidOperationException(
            $"Service of type '{typeof(T).Name}' is not registered. " +
            $"Call ServiceLocator.Register<{typeof(T).Name}>() during initialization.");
    }

    /// <summary>
    /// Try to resolve a service. Returns false if not registered.
    /// </summary>
    public static bool TryGet<T>(out T? service) where T : notnull
    {
        if (_services.TryGetValue(typeof(T), out var obj))
        {
            service = (T)obj;
            return true;
        }

        service = default;
        return false;
    }

    /// <summary>
    /// Remove all registered services. Called during shutdown.
    /// </summary>
    public static void Clear()
    {
        _services.Clear();
    }
}
