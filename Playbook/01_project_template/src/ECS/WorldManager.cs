using Arch.Core;
using Microsoft.Xna.Framework;

namespace MyGame.ECS;

/// <summary>
/// Manages an Arch ECS <see cref="World"/> lifecycle and system execution.
/// Each scene typically owns one WorldManager instance.
/// </summary>
/// <remarks>
/// Systems in Arch are plain methods — no base class required.
/// Register them as delegates and WorldManager will invoke them each frame.
/// </remarks>
public class WorldManager : IDisposable
{
    /// <summary>The Arch ECS world instance.</summary>
    public World World { get; }

    private readonly List<Action<World, GameTime>> _updateSystems = new();
    private readonly List<Action<World, GameTime>> _drawSystems = new();
    private bool _disposed;

    /// <summary>
    /// Create a new WorldManager with a fresh Arch World.
    /// </summary>
    public WorldManager()
    {
        World = World.Create();
    }

    /// <summary>
    /// Register a system that runs during the Update phase.
    /// </summary>
    /// <param name="system">A method that takes the World and GameTime.</param>
    public void AddUpdateSystem(Action<World, GameTime> system)
    {
        _updateSystems.Add(system);
    }

    /// <summary>
    /// Register a system that runs during the Draw phase.
    /// </summary>
    /// <param name="system">A method that takes the World and GameTime.</param>
    public void AddDrawSystem(Action<World, GameTime> system)
    {
        _drawSystems.Add(system);
    }

    /// <summary>
    /// Run all registered update systems in order.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        foreach (var system in _updateSystems)
        {
            system(World, gameTime);
        }
    }

    /// <summary>
    /// Run all registered draw systems in order.
    /// </summary>
    public void Draw(GameTime gameTime)
    {
        foreach (var system in _drawSystems)
        {
            system(World, gameTime);
        }
    }

    /// <summary>
    /// Dispose the Arch World and release resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        World.Dispose();
        _updateSystems.Clear();
        _drawSystems.Clear();

        GC.SuppressFinalize(this);
    }
}
