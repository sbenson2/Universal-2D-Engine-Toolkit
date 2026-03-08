using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Simple combat resolution. Calculates damage from attack vs defense,
/// applies damage, handles death and entity removal.
/// </summary>
public sealed class CombatSystem
{
    /// <summary>
    /// Resolve a melee attack from attacker to defender.
    /// Returns damage dealt and whether the defender died.
    /// </summary>
    public (int Damage, bool Killed) Attack(
        Entity attacker, Entity defender, MessageLog log)
    {
        ref var attackerStats = ref attacker.Get<Stats>();
        ref var defenderStats = ref defender.Get<Stats>();

        int damage = RoguelikeConfig.CalculateDamage(attackerStats.Attack, defenderStats.Defense);
        defenderStats = defenderStats with { Hp = defenderStats.Hp - damage };

        string attackerName = attacker.Has<PlayerTag>() ? "You" : GetEntityName(attacker);
        string defenderName = defender.Has<PlayerTag>() ? "you" : GetEntityName(defender);
        string verb = attacker.Has<PlayerTag>() ? "hit" : "hits";

        log.Add($"{attackerName} {verb} {defenderName} for {damage} damage.",
            attacker.Has<PlayerTag>() ? Color.LightGreen : Color.LightCoral);

        if (defenderStats.Hp <= 0)
        {
            string deathName = defender.Has<PlayerTag>() ? "You die!" : $"The {defenderName} dies!";
            log.Add(deathName, Color.Red);
            return (damage, true);
        }

        return (damage, false);
    }

    /// <summary>
    /// Award EXP to the killer and check for level-up.
    /// </summary>
    public void AwardExp(Entity killer, int expValue, MessageLog log)
    {
        if (!killer.Has<Stats>()) return;
        ref var stats = ref killer.Get<Stats>();

        stats = stats with { Exp = stats.Exp + expValue };

        while (stats.Exp >= stats.ExpToNext)
        {
            stats = stats with
            {
                Level = stats.Level + 1,
                Exp = stats.Exp - stats.ExpToNext,
                ExpToNext = RoguelikeConfig.ExpForLevel(stats.Level + 1),
                MaxHp = stats.MaxHp + 5,
                Hp = stats.MaxHp + 5, // Full heal on level up
                Attack = stats.Attack + 1,
                Defense = stats.Defense + 1
            };

            log.Add($"Welcome to level {stats.Level}!", Color.Yellow);
        }
    }

    private static string GetEntityName(Entity entity)
    {
        // Simple name resolution based on tags — extend as needed
        if (entity.Has<EnemyTag>()) return "enemy";
        if (entity.Has<ItemTag>()) return "item";
        return "something";
    }
}
