namespace MyGame.Roguelike.Components;

/// <summary>
/// Tracks turn-based action timing via an energy system.
/// Each tick, <see cref="EnergyPerTurn"/> is added to <see cref="Energy"/>.
/// When Energy reaches the threshold (100), the entity may act.
/// <see cref="Speed"/> influences <see cref="EnergyPerTurn"/> calculation during spawning.
/// </summary>
public record struct TurnActor(int Speed, int Energy, int EnergyPerTurn);
