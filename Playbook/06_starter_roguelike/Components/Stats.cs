namespace MyGame.Roguelike.Components;

/// <summary>
/// Core RPG stats for any combatant (player or enemy).
/// </summary>
public record struct Stats(
    int MaxHp,
    int Hp,
    int Attack,
    int Defense,
    int Level,
    int Exp,
    int ExpToNext);
