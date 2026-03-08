namespace MyGame.TopDown.Components;

/// <summary>
/// Core RPG stats for a character entity. Covers health, combat, and progression.
/// Attach to both the player and NPCs/enemies that participate in combat.
/// </summary>
/// <param name="MaxHp">Maximum hit points.</param>
/// <param name="Hp">Current hit points.</param>
/// <param name="Attack">Base attack power.</param>
/// <param name="Defense">Base defense / damage reduction.</param>
/// <param name="Speed">Turn order / action speed.</param>
/// <param name="Level">Current character level.</param>
/// <param name="Exp">Accumulated experience points.</param>
public record struct Stats(int MaxHp, int Hp, int Attack, int Defense, int Speed, int Level, int Exp);
