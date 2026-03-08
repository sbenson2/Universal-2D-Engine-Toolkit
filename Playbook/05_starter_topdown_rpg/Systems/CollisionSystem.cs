using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.TopDown.Components;
using MyGame.TopDown.Tags;

namespace MyGame.TopDown.Systems;

/// <summary>
/// AABB slide collision system. Tests all entities with <see cref="CharacterBody"/> and
/// <see cref="Velocity"/> against all <see cref="SolidTag"/> entities.
/// Uses the "try X then Y" slide approach: move on X axis first, resolve,
/// then move on Y axis, resolve. This lets entities slide along walls instead of stopping dead.
/// </summary>
/// <remarks>
/// Collision boxes are centered on the entity's <see cref="Position"/> and sized by <see cref="CharacterBody"/>.
/// In a 3/4 top-down view, the body should represent the ground footprint (feet area),
/// NOT the full sprite — typically ~12×8 px for a 16×32 character sprite.
/// </remarks>
public static class CollisionSystem
{
    private static readonly QueryDescription MoverQuery = new QueryDescription()
        .WithAll<Position, Velocity, CharacterBody>();

    private static readonly QueryDescription SolidQuery = new QueryDescription()
        .WithAll<Position, CharacterBody, SolidTag>();

    /// <summary>
    /// Register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// Runs AFTER <see cref="TopDownMovementSystem"/> to push movers out of solid overlaps.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        // Collect solids into a list for inner-loop checks.
        // For large worlds, replace with a spatial hash.
        var solids = new List<(float X, float Y, float W, float H)>();

        world.Query(in SolidQuery, (ref Position sp, ref CharacterBody sb) =>
        {
            solids.Add((sp.X, sp.Y, sb.Width, sb.Height));
        });

        if (solids.Count == 0) return;

        world.Query(in MoverQuery, (Entity entity, ref Position pos, ref Velocity vel, ref CharacterBody body) =>
        {
            // Skip solids colliding with themselves.
            if (world.Has<SolidTag>(entity)) return;

            float hw = body.Width * 0.5f;
            float hh = body.Height * 0.5f;

            // --- Resolve X axis ---
            foreach (var (sx, sy, sw, sh) in solids)
            {
                if (AabbOverlap(pos.X, pos.Y, hw, hh, sx, sy, sw * 0.5f, sh * 0.5f))
                {
                    // Push out on X.
                    float overlapX = (hw + sw * 0.5f) - MathF.Abs(pos.X - sx);
                    if (pos.X < sx)
                        pos = pos with { X = pos.X - overlapX };
                    else
                        pos = pos with { X = pos.X + overlapX };

                    vel = vel with { Dx = 0f };
                }
            }

            // --- Resolve Y axis ---
            foreach (var (sx, sy, sw, sh) in solids)
            {
                if (AabbOverlap(pos.X, pos.Y, hw, hh, sx, sy, sw * 0.5f, sh * 0.5f))
                {
                    float overlapY = (hh + sh * 0.5f) - MathF.Abs(pos.Y - sy);
                    if (pos.Y < sy)
                        pos = pos with { Y = pos.Y - overlapY };
                    else
                        pos = pos with { Y = pos.Y + overlapY };

                    vel = vel with { Dy = 0f };
                }
            }
        });
    }

    /// <summary>
    /// Tests overlap between two center-based AABBs.
    /// </summary>
    private static bool AabbOverlap(
        float ax, float ay, float ahw, float ahh,
        float bx, float by, float bhw, float bhh)
    {
        return MathF.Abs(ax - bx) < (ahw + bhw)
            && MathF.Abs(ay - by) < (ahh + bhh);
    }
}
