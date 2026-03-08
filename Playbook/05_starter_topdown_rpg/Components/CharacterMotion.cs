namespace MyGame.TopDown.Components;

/// <summary>
/// Motion tuning for a movable character.
/// Acceleration ramps up velocity toward <see cref="MoveSpeed"/>;
/// Friction decelerates when no input is applied.
/// </summary>
/// <param name="MoveSpeed">Maximum movement speed in pixels/second.</param>
/// <param name="Acceleration">How quickly the entity reaches max speed (pixels/sec²).</param>
/// <param name="Friction">Deceleration applied when no input (pixels/sec²).</param>
public record struct CharacterMotion(float MoveSpeed, float Acceleration, float Friction);
