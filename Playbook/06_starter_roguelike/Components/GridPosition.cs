namespace MyGame.Roguelike.Components;

/// <summary>
/// Tile-based grid position. All movement in the roguelike is tile-to-tile.
/// </summary>
public record struct GridPosition(int X, int Y);
