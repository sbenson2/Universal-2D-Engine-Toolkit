using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.Platformer.Components;

namespace MyGame.Platformer.Systems;

/// <summary>
/// Handles jump initiation with coyote time and jump buffering.
/// <list type="bullet">
///   <item><b>Coyote time</b> — allows jumping for a few frames after walking off a ledge.</item>
///   <item><b>Jump buffering</b> — remembers a jump press for a few frames before landing.</item>
/// </list>
/// Together these solve the two most common "the game ate my input" complaints.
/// See G52 §6–§7 for the full rationale.
/// </summary>
public static class JumpSystem
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
            // ── Tick timers ──
            if (!body.IsGrounded)
                body = body with { CoyoteTimer = MathF.Max(body.CoyoteTimer - dt, 0f) };

            if (intent.JumpPressed)
                body = body with { JumpBufferTimer = motion.JumpBufferTime };
            else
                body = body with { JumpBufferTimer = MathF.Max(body.JumpBufferTimer - dt, 0f) };

            // ── Can we jump? ──
            bool wantsJump = intent.JumpPressed || body.JumpBufferTimer > 0f;
            bool canJump = body.IsGrounded || body.CoyoteTimer > 0f;

            if (wantsJump && canJump)
            {
                // Launch upward (negative Y = up in screen space).
                vel = vel with { Dy = -motion.JumpForce };

                // Consume timers so we don't double-jump.
                body = body with
                {
                    IsGrounded = false,
                    CoyoteTimer = 0f,
                    JumpBufferTimer = 0f
                };
            }
        });
    }
}
