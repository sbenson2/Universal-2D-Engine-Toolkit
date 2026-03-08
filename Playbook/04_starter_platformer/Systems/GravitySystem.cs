using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.Platformer.Components;

namespace MyGame.Platformer.Systems;

/// <summary>
/// Applies gravity to all entities with a <see cref="CharacterMotion"/> component.
/// Uses a fall-gravity multiplier so descents feel snappier than ascents —
/// the classic "fast fall" trick used by Celeste, Hollow Knight, and nearly
/// every great platformer (see G52 §5).
/// </summary>
/// <remarks>
/// Variable jump height: when the player releases the jump button while ascending,
/// gravity is increased to cut the arc short. This gives one button both
/// short hops and full jumps.
/// </remarks>
public static class GravitySystem
{
    private static readonly QueryDescription Query = new QueryDescription()
        .WithAll<Velocity, CharacterBody, CharacterMotion, PlayerIntent>();

    /// <summary>
    /// Update method — register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        world.Query(in Query, (
            ref Velocity vel,
            ref CharacterBody body,
            ref CharacterMotion motion,
            ref PlayerIntent intent) =>
        {
            // Don't apply gravity while grounded.
            if (body.IsGrounded) return;

            float gravity = motion.Gravity;

            if (vel.Dy > 0f)
            {
                // Falling — heavier gravity for snappy descent.
                gravity *= motion.FallGravityMultiplier;
            }
            else if (vel.Dy < 0f && !intent.JumpHeld)
            {
                // Ascending but jump released — cut the arc (variable jump height).
                gravity *= motion.FallGravityMultiplier;
            }

            float newDy = vel.Dy + gravity * dt;
            newDy = MathF.Min(newDy, motion.MaxFallSpeed);

            vel = vel with { Dy = newDy };
        });
    }
}
