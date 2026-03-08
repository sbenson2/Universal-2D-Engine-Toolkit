namespace MyGame.TopDown.Components;

/// <summary>
/// The direction an entity is currently facing.
/// Values are -1, 0, or 1 on each axis. Supports 4-directional (one axis zero)
/// and 8-directional (both axes nonzero for diagonals).
/// Default facing is down: (0, 1).
/// </summary>
/// <param name="X">Horizontal facing: -1 = left, 0 = neutral, 1 = right.</param>
/// <param name="Y">Vertical facing: -1 = up, 0 = neutral, 1 = down.</param>
public record struct FacingDirection(int X, int Y);
