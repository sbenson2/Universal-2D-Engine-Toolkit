using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;

namespace MyGame.ECS.Systems;

/// <summary>
/// Moves all entities that have both Position and Velocity components.
/// Applies velocity scaled by delta time each frame.
/// </summary>
public static class MovementSystem
{
    private static readonly QueryDescription Query = new QueryDescription()
        .WithAll<Position, Velocity>();

    /// <summary>
    /// Update method to register with <see cref="WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        world.Query(in Query, (ref Position pos, ref Velocity vel) =>
        {
            pos = pos with
            {
                X = pos.X + vel.Dx * dt,
                Y = pos.Y + vel.Dy * dt
            };
        });
    }
}
