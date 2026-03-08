using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.Platformer.Components;
using MyGame.Platformer.Tags;

namespace MyGame.Platformer.Systems;

/// <summary>
/// AABB ground detection: checks if character feet overlap any ground tile,
/// sets <see cref="CharacterBody.IsGrounded"/>, and starts the coyote timer
/// when the character walks off an edge.
/// </summary>
/// <remarks>
/// This is intentionally simple — a downward overlap test from the character's
/// bottom edge against all ground AABBs. For a full tilemap you'd spatial-hash
/// nearby tiles first; for a starter kit, brute-force against spawned platforms works.
/// </remarks>
public static class GroundDetectionSystem
{
    /// <summary>How far below the character's feet to probe for ground (pixels).</summary>
    private const float GroundCheckDepth = 2f;

    private static readonly QueryDescription GroundQuery = new QueryDescription()
        .WithAll<Position, CharacterBody, CharacterMotion, Velocity>();

    private static readonly QueryDescription TileQuery = new QueryDescription()
        .WithAll<Position, GroundTag>();

    // Reusable list to avoid per-frame allocation.
    private static readonly List<Rectangle> _groundRects = new();

    /// <summary>
    /// Update method — register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        // ── 1. Gather all ground AABBs ──
        _groundRects.Clear();
        world.Query(in TileQuery, (ref Position tilePos) =>
        {
            // Ground tiles are assumed to be PlatformerConfig.TileSize × PlatformerConfig.TileSize,
            // positioned at their top-left corner.
            int ts = PlatformerConfig.TileSize;
            _groundRects.Add(new Rectangle(
                (int)tilePos.X, (int)tilePos.Y, ts, ts));
        });

        // ── 2. Test each character against ground ──
        world.Query(in GroundQuery, (
            ref Position pos,
            ref CharacterBody body,
            ref CharacterMotion motion,
            ref Velocity vel) =>
        {
            bool wasGrounded = body.IsGrounded;
            bool grounded = false;

            // Character feet rectangle: a thin strip at the bottom of the collider.
            float feetLeft = pos.X - body.Width * 0.5f;
            float feetRight = pos.X + body.Width * 0.5f;
            float feetTop = pos.Y + body.Height * 0.5f;      // bottom of the character
            float feetBottom = feetTop + GroundCheckDepth;

            foreach (var tile in _groundRects)
            {
                // AABB overlap test.
                if (feetRight > tile.Left &&
                    feetLeft < tile.Right &&
                    feetBottom > tile.Top &&
                    feetTop <= tile.Top + GroundCheckDepth)
                {
                    grounded = true;

                    // Snap feet to tile surface to prevent sinking.
                    if (vel.Dy >= 0f)
                    {
                        pos = pos with { Y = tile.Top - body.Height * 0.5f };
                        vel = vel with { Dy = 0f };
                    }
                    break;
                }
            }

            // ── Coyote timer management ──
            if (grounded && !wasGrounded)
            {
                // Just landed — reset coyote timer.
                body = body with
                {
                    IsGrounded = true,
                    WasGrounded = wasGrounded,
                    CoyoteTimer = motion.CoyoteTime
                };
            }
            else if (!grounded && wasGrounded)
            {
                // Just left ground — start coyote countdown.
                body = body with
                {
                    IsGrounded = false,
                    WasGrounded = wasGrounded,
                    CoyoteTimer = motion.CoyoteTime
                };
            }
            else
            {
                body = body with
                {
                    IsGrounded = grounded,
                    WasGrounded = wasGrounded
                };
            }
        });
    }
}
