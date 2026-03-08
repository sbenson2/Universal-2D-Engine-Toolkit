namespace MyGame.Roguelike.Components;

/// <summary>
/// Tracks which tiles an entity can currently see.
/// <see cref="Radius"/> is the vision distance; <see cref="VisibleTiles"/> is updated
/// each turn by the FOV system using recursive shadowcasting.
/// </summary>
public record struct FieldOfView(int Radius, HashSet<(int X, int Y)> VisibleTiles);
