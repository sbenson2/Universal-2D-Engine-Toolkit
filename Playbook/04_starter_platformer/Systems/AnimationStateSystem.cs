using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.Platformer.Components;

namespace MyGame.Platformer.Systems;

/// <summary>
/// Sets the <see cref="AnimationState"/> based on current velocity and grounded state.
/// Produces one of four animation keys: <c>"idle"</c>, <c>"run"</c>, <c>"jump"</c>, <c>"fall"</c>.
/// Hook your sprite animator up to these keys for automatic state transitions.
/// </summary>
public static class AnimationStateSystem
{
    /// <summary>Minimum horizontal speed to count as "running" (avoids flickering at low speeds).</summary>
    private const float RunThreshold = 10f;

    private static readonly QueryDescription Query = new QueryDescription()
        .WithAll<AnimationState, Velocity, CharacterBody, FacingDirection>();

    /// <summary>
    /// Update method — register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        world.Query(in Query, (
            ref AnimationState anim,
            ref Velocity vel,
            ref CharacterBody body,
            ref FacingDirection facing) =>
        {
            string newAnim;

            if (!body.IsGrounded)
            {
                // Airborne: jump (ascending) or fall (descending).
                newAnim = vel.Dy < 0f ? "jump" : "fall";
            }
            else if (MathF.Abs(vel.Dx) > RunThreshold)
            {
                newAnim = "run";
            }
            else
            {
                newAnim = "idle";
            }

            bool flipX = facing.Direction < 0;

            anim = new AnimationState(newAnim, flipX);
        });
    }
}
