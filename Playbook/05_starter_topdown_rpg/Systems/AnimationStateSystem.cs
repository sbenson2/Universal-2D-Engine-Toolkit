using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.TopDown.Components;

namespace MyGame.TopDown.Systems;

/// <summary>
/// Derives the current animation name from velocity and facing direction.
/// Produces standard animation keys: idle_down, idle_up, idle_side,
/// walk_down, walk_up, walk_side. The <see cref="AnimationState.FlipX"/> flag
/// handles left vs right by mirroring the "side" animations.
/// </summary>
/// <remarks>
/// Convention: art provides 3 directional sets (down, up, side-right).
/// Left-facing reuses side-right with FlipX = true.
/// </remarks>
public static class AnimationStateSystem
{
    private static readonly QueryDescription Query = new QueryDescription()
        .WithAll<Velocity, FacingDirection, AnimationState>();

    /// <summary>Speed threshold below which the entity plays idle animations.</summary>
    private const float IdleThreshold = 5f;

    /// <summary>
    /// Register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        world.Query(in Query, (ref Velocity vel, ref FacingDirection facing, ref AnimationState anim) =>
        {
            float speed = MathF.Sqrt(vel.Dx * vel.Dx + vel.Dy * vel.Dy);
            bool isMoving = speed > IdleThreshold;

            string prefix = isMoving ? "walk" : "idle";

            // Determine direction suffix from facing.
            // Priority: if facing has a vertical component only → up/down.
            // If facing has a horizontal component → side.
            // If both → prefer vertical for RPG feel (Zelda-style).
            string suffix;
            bool flipX = false;

            if (facing.X == 0 || (facing.Y != 0 && facing.X != 0))
            {
                // Vertical dominant or pure vertical.
                suffix = facing.Y < 0 ? "up" : "down";
            }
            else
            {
                // Pure horizontal.
                suffix = "side";
                flipX = facing.X < 0;
            }

            string newAnim = $"{prefix}_{suffix}";

            anim = new AnimationState(newAnim, flipX);
        });
    }
}
