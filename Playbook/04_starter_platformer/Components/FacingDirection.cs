namespace MyGame.Platformer.Components;

/// <summary>
/// Tracks which horizontal direction the character is facing.
/// Used by animation and rendering systems to flip sprites.
/// </summary>
/// <param name="Direction">1 = right, -1 = left.</param>
public record struct FacingDirection(int Direction);
