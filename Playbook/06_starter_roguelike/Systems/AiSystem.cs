using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Simple enemy AI. Determines behavior each turn:
/// - <see cref="AiBehavior.Wander"/>: random movement when player not in sight
/// - <see cref="AiBehavior.Chase"/>: move toward the player when visible
/// - <see cref="AiBehavior.Flee"/>: move away from the player when HP is low
/// </summary>
public sealed class AiSystem
{
    private static readonly QueryDescription EnemyQuery =
        new QueryDescription().WithAll<GridPosition, AiIntent, Stats, EnemyTag, TurnActor>();

    private static readonly QueryDescription PlayerQuery =
        new QueryDescription().WithAll<GridPosition, PlayerTag, FieldOfView>();

    private readonly Random _rng = new();

    /// <summary>
    /// Process AI decisions for the current actor. Returns a movement delta.
    /// </summary>
    public (int DX, int DY) DecideMove(
        World world, Entity enemy, GridPosition playerPos, HashSet<(int, int)> playerFov)
    {
        ref var pos = ref enemy.Get<GridPosition>();
        ref var stats = ref enemy.Get<Stats>();
        ref var intent = ref enemy.Get<AiIntent>();

        bool playerCanSee = playerFov.Contains((pos.X, pos.Y));

        // Determine behavior
        float hpRatio = stats.MaxHp > 0 ? (float)stats.Hp / stats.MaxHp : 1f;

        if (hpRatio <= RoguelikeConfig.AiFleeThreshold)
        {
            intent = new AiIntent(AiBehavior.Flee);
        }
        else if (playerCanSee)
        {
            intent = new AiIntent(AiBehavior.Chase);
        }
        else
        {
            intent = new AiIntent(AiBehavior.Wander);
        }

        return intent.Behavior switch
        {
            AiBehavior.Chase => MoveToward(pos, playerPos),
            AiBehavior.Flee => MoveAway(pos, playerPos),
            AiBehavior.Wander => RandomMove(),
            _ => (0, 0)
        };
    }

    private static (int, int) MoveToward(GridPosition from, GridPosition to)
    {
        int dx = Math.Sign(to.X - from.X);
        int dy = Math.Sign(to.Y - from.Y);

        // Prefer the axis with greater distance
        if (Math.Abs(to.X - from.X) >= Math.Abs(to.Y - from.Y))
            return (dx, 0);
        else
            return (0, dy);
    }

    private static (int, int) MoveAway(GridPosition from, GridPosition to)
    {
        var (dx, dy) = MoveToward(from, to);
        return (-dx, -dy);
    }

    private (int, int) RandomMove()
    {
        ReadOnlySpan<(int, int)> directions = stackalloc (int, int)[]
        {
            (0, -1), (0, 1), (-1, 0), (1, 0), (0, 0)
        };
        return directions[_rng.Next(directions.Length)];
    }
}
