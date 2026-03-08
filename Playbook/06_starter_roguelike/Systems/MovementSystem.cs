using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Map;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Processes movement on the grid. Checks walkability and handles bump-attacks
/// (moving into a tile occupied by an enemy triggers combat).
/// </summary>
public sealed class MovementSystem
{
    private static readonly QueryDescription BlockerQuery =
        new QueryDescription().WithAll<GridPosition, BlocksMovementTag>();

    /// <summary>
    /// Result of attempting to move an entity.
    /// </summary>
    public enum MoveResult
    {
        /// <summary>Entity moved successfully.</summary>
        Moved,
        /// <summary>Tile is not walkable (wall, out of bounds).</summary>
        Blocked,
        /// <summary>Tile is occupied by another entity — triggers a bump attack.</summary>
        BumpAttack
    }

    /// <summary>
    /// Attempt to move an entity by (dx, dy). Returns the result and the
    /// bumped entity if applicable.
    /// </summary>
    public (MoveResult Result, Entity? BumpTarget) TryMove(
        World world, Entity entity, int dx, int dy, GameMap map)
    {
        ref var pos = ref entity.Get<GridPosition>();
        int newX = pos.X + dx;
        int newY = pos.Y + dy;

        // Check map walkability
        if (!map.IsWalkable(newX, newY))
            return (MoveResult.Blocked, null);

        // Check for blocking entities at the target position
        Entity? blocker = null;
        world.Query(in BlockerQuery, (Entity other, ref GridPosition otherPos) =>
        {
            if (other != entity && otherPos.X == newX && otherPos.Y == newY)
            {
                blocker = other;
            }
        });

        if (blocker.HasValue)
        {
            // If the blocker has Stats, it's a bump-attack target
            if (blocker.Value.Has<Stats>())
                return (MoveResult.BumpAttack, blocker);
            else
                return (MoveResult.Blocked, null);
        }

        // Move the entity
        pos = new GridPosition(newX, newY);
        return (MoveResult.Moved, null);
    }
}
