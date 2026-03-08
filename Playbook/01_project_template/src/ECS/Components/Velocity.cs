namespace MyGame.ECS.Components;

/// <summary>
/// 2D velocity vector applied per frame during movement.
/// </summary>
public record struct Velocity(float Dx, float Dy);
