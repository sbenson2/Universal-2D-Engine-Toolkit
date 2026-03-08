using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.Platformer.Components;

namespace MyGame.Platformer.Systems;

/// <summary>
/// Applies horizontal acceleration and friction to character entities.
/// Uses separate ground vs air parameters for the "committed trajectory" feel:
/// high ground accel/friction for crisp stops, lower air values for momentum.
/// </summary>
/// <remarks>
/// Philosophy (from G52): We set velocity directly — no AddForce, no rigid body.
/// Kinematic control means every frame of movement is intentional.
/// </remarks>
public static class CharacterMovementSystem
{
    private static readonly QueryDescription Query = new QueryDescription()
        .WithAll<Velocity, CharacterBody, CharacterMotion, PlayerIntent, FacingDirection>();

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
            ref PlayerIntent intent,
            ref FacingDirection facing) =>
        {
            float accel = body.IsGrounded ? motion.Acceleration : motion.AirAcceleration;
            float friction = body.IsGrounded ? motion.Friction : motion.AirFriction;

            if (MathF.Abs(intent.MoveX) > 0.01f)
            {
                // Update facing direction.
                facing = new FacingDirection(intent.MoveX > 0 ? 1 : -1);

                // Accelerate toward target speed.
                float target = intent.MoveX * motion.MoveSpeed;
                vel = vel with { Dx = MoveToward(vel.Dx, target, accel * dt) };
            }
            else
            {
                // No input — apply friction to decelerate.
                vel = vel with { Dx = MoveToward(vel.Dx, 0f, friction * dt) };
            }
        });
    }

    /// <summary>
    /// Moves <paramref name="current"/> toward <paramref name="target"/> by at most
    /// <paramref name="maxDelta"/>, without overshooting.
    /// </summary>
    private static float MoveToward(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
            return target;
        return current + MathF.Sign(target - current) * maxDelta;
    }
}
