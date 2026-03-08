using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Energy-based turn system. Each tick, all actors accumulate energy.
/// When an actor reaches the energy threshold, it may act.
/// The player's turn pauses the system until input is received.
/// </summary>
public sealed class TurnSystem
{
    private static readonly QueryDescription ActorQuery =
        new QueryDescription().WithAll<TurnActor, GridPosition>();

    private static readonly QueryDescription PlayerQuery =
        new QueryDescription().WithAll<TurnActor, PlayerTag>();

    /// <summary>True when the system is waiting for the player to provide input.</summary>
    public bool WaitingForInput { get; set; }

    /// <summary>True when it's the player's turn and they haven't acted yet.</summary>
    public bool IsPlayerTurn { get; private set; }

    /// <summary>The entity that should act this tick, if any.</summary>
    public Entity? CurrentActor { get; private set; }

    /// <summary>
    /// Advance the turn system. Call each frame from Update.
    /// </summary>
    public void Update(World world, GameTime gameTime)
    {
        // If waiting for input, don't advance
        if (WaitingForInput) return;

        // Find the next actor with enough energy
        CurrentActor = null;
        IsPlayerTurn = false;

        // Give energy to all actors
        world.Query(in ActorQuery, (Entity entity, ref TurnActor actor) =>
        {
            actor = actor with { Energy = actor.Energy + actor.EnergyPerTurn };
        });

        // Find first actor ready to act (energy >= threshold)
        // Player gets priority if they have enough energy
        Entity? readyPlayer = null;
        Entity? readyEnemy = null;

        world.Query(in ActorQuery, (Entity entity, ref TurnActor actor) =>
        {
            if (actor.Energy < RoguelikeConfig.EnergyThreshold) return;

            if (entity.Has<PlayerTag>())
            {
                readyPlayer ??= entity;
            }
            else
            {
                readyEnemy ??= entity;
            }
        });

        // Player goes first
        if (readyPlayer.HasValue)
        {
            CurrentActor = readyPlayer;
            IsPlayerTurn = true;
            WaitingForInput = true;
        }
        else if (readyEnemy.HasValue)
        {
            CurrentActor = readyEnemy;
            IsPlayerTurn = false;
        }
    }

    /// <summary>
    /// Consume energy after an actor takes their action.
    /// </summary>
    public void ConsumeEnergy(Entity entity)
    {
        if (!entity.IsAlive()) return;
        ref var actor = ref entity.Get<TurnActor>();
        actor = actor with { Energy = actor.Energy - RoguelikeConfig.EnergyThreshold };
        WaitingForInput = false;
    }
}
