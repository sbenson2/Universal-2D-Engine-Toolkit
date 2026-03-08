namespace MyGame.Platformer.Components;

/// <summary>
/// Movement tuning parameters for a kinematic character controller.
/// Separates ground and air physics for the "committed air trajectory" feel
/// that makes great platformers tick (see G52: game feel first, physics second).
/// </summary>
/// <param name="MoveSpeed">Maximum horizontal speed in pixels/second.</param>
/// <param name="Acceleration">Ground horizontal acceleration in pixels/second².</param>
/// <param name="Friction">Ground deceleration when no input, in pixels/second².</param>
/// <param name="AirAcceleration">Horizontal acceleration while airborne, in pixels/second².</param>
/// <param name="AirFriction">Horizontal deceleration while airborne with no input, in pixels/second².</param>
/// <param name="JumpForce">Initial upward velocity on jump, in pixels/second (negative Y = up).</param>
/// <param name="Gravity">Downward acceleration in pixels/second². Derived from jump height and time-to-apex.</param>
/// <param name="FallGravityMultiplier">Gravity scale when falling (typically 1.5–2.5 for snappy descent).</param>
/// <param name="MaxFallSpeed">Terminal velocity cap in pixels/second.</param>
/// <param name="CoyoteTime">Grace period (seconds) after leaving ground where jump is still allowed.</param>
/// <param name="JumpBufferTime">Window (seconds) before landing where a jump press is remembered.</param>
public record struct CharacterMotion(
    float MoveSpeed,
    float Acceleration,
    float Friction,
    float AirAcceleration,
    float AirFriction,
    float JumpForce,
    float Gravity,
    float FallGravityMultiplier,
    float MaxFallSpeed,
    float CoyoteTime,
    float JumpBufferTime);
