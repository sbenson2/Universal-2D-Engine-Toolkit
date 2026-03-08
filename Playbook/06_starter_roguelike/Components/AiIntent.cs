namespace MyGame.Roguelike.Components;

/// <summary>
/// AI behavioral modes for enemies.
/// </summary>
public enum AiBehavior
{
    /// <summary>Move randomly when idle.</summary>
    Wander,
    /// <summary>Pursue the player when in line of sight.</summary>
    Chase,
    /// <summary>Run away when HP is low.</summary>
    Flee
}

/// <summary>
/// Indicates an entity's current AI behavior intent.
/// Updated each turn by <see cref="Systems.AiSystem"/>.
/// </summary>
public record struct AiIntent(AiBehavior Behavior);
