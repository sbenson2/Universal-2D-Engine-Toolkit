using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.TopDown.Components;

namespace MyGame.TopDown.Systems;

/// <summary>
/// Applies velocity to position each frame with acceleration ramping and friction.
/// Runs AFTER <see cref="InputSystem"/> (which sets target velocity)
/// and BEFORE <see cref="CollisionSystem"/> (which resolves overlaps).
/// </summary>
/// <remarks>
/// Movement uses a simple approach: InputSystem already sets velocity to desired direction × speed.
/// This system applies that velocity to position, scaled by delta time.
/// For smoother feel, swap in acceleration/friction interpolation.
/// </remarks>
public static class TopDownMovementSystem
{
    private static readonly QueryDescription Query = new QueryDescription()
        .WithAll<Position, Velocity>();

    /// <summary>
    /// Register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// Integrates velocity into position (velocity × dt).
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
