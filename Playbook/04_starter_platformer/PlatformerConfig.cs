using MyGame.Platformer.Components;

namespace MyGame.Platformer;

/// <summary>
/// Central tuning constants for the platformer starter kit.
/// Tweak these first — they're the "designer knobs" that control game feel.
/// All physics values are derived from jump height and time-to-apex (G52 §1: game feel first).
/// </summary>
public static class PlatformerConfig
{
    // ════════════════════════════════════════════
    //  JUMP FEEL (primary inputs — everything else derives from these)
    // ════════════════════════════════════════════

    /// <summary>Desired jump apex height in pixels.</summary>
    public const float JumpHeight = 72f;

    /// <summary>Seconds to reach the apex of a full jump.</summary>
    public const float TimeToApex = 0.35f;

    // Derived physics (from kinematic equations):
    //   gravity      = 2 * jumpHeight / timeToApex²
    //   jumpVelocity = 2 * jumpHeight / timeToApex

    /// <summary>Gravity in pixels/second². Derived: 2 * JumpHeight / TimeToApex².</summary>
    public static readonly float Gravity = 2f * JumpHeight / (TimeToApex * TimeToApex);

    /// <summary>Initial upward velocity on jump. Derived: 2 * JumpHeight / TimeToApex.</summary>
    public static readonly float JumpForce = 2f * JumpHeight / TimeToApex;

    // ════════════════════════════════════════════
    //  FALL FEEL
    // ════════════════════════════════════════════

    /// <summary>
    /// Gravity multiplier when falling (vel.Y > 0) or when jump is released early.
    /// Higher = snappier descent. Celeste ≈ 2.5, Ori ≈ 1.4. Default: 2.0.
    /// </summary>
    public const float FallGravityMultiplier = 2.0f;

    /// <summary>Terminal velocity cap in pixels/second. Prevents infinite fall speed.</summary>
    public const float MaxFallSpeed = 400f;

    // ════════════════════════════════════════════
    //  HORIZONTAL MOVEMENT
    // ════════════════════════════════════════════

    /// <summary>Max horizontal speed in pixels/second.</summary>
    public const float MoveSpeed = 200f;

    /// <summary>Ground acceleration in pixels/second². Higher = snappier starts.</summary>
    public const float Acceleration = 1800f;

    /// <summary>Ground friction (deceleration) in pixels/second². Higher = crisper stops.</summary>
    public const float Friction = 2400f;

    /// <summary>Air acceleration in pixels/second². Lower than ground = committed air trajectory.</summary>
    public const float AirAcceleration = 1200f;

    /// <summary>Air friction in pixels/second². Low = preserves momentum while airborne.</summary>
    public const float AirFriction = 600f;

    // ════════════════════════════════════════════
    //  FORGIVENESS WINDOWS
    // ════════════════════════════════════════════

    /// <summary>
    /// Coyote time in seconds — how long after walking off a ledge you can still jump.
    /// 0.1s ≈ 6 frames at 60fps. Named after Wile E. Coyote. Essential for feel.
    /// </summary>
    public const float CoyoteTime = 0.1f;

    /// <summary>
    /// Jump buffer time in seconds — how early before landing a jump press is remembered.
    /// 0.133s ≈ 8 frames at 60fps. Prevents "the game ate my input" complaints.
    /// </summary>
    public const float JumpBufferTime = 0.133f;

    // ════════════════════════════════════════════
    //  CAMERA
    // ════════════════════════════════════════════

    /// <summary>Deadzone width in pixels — camera won't move until player exits this box.</summary>
    public const float CameraDeadzoneWidth = 48f;

    /// <summary>Deadzone height in pixels.</summary>
    public const float CameraDeadzoneHeight = 32f;

    /// <summary>Pixels to shift the camera ahead of the player in their facing direction.</summary>
    public const float CameraLookahead = 40f;

    /// <summary>Camera interpolation speed (higher = less lag, 1.0 = instant).</summary>
    public const float CameraSmoothSpeed = 5f;

    // ════════════════════════════════════════════
    //  WORLD
    // ════════════════════════════════════════════

    /// <summary>Tile size in pixels. Ground tiles use this as both width and height.</summary>
    public const int TileSize = 16;

    /// <summary>Player collision box width in pixels.</summary>
    public const float PlayerWidth = 14f;

    /// <summary>Player collision box height in pixels.</summary>
    public const float PlayerHeight = 24f;

    // ════════════════════════════════════════════
    //  HELPER
    // ════════════════════════════════════════════

    /// <summary>
    /// Creates a <see cref="CharacterMotion"/> component pre-filled with the config values.
    /// Call this when spawning a player entity.
    /// </summary>
    public static CharacterMotion DefaultMotion() => new(
        MoveSpeed:              MoveSpeed,
        Acceleration:           Acceleration,
        Friction:               Friction,
        AirAcceleration:        AirAcceleration,
        AirFriction:            AirFriction,
        JumpForce:              JumpForce,
        Gravity:                Gravity,
        FallGravityMultiplier:  FallGravityMultiplier,
        MaxFallSpeed:           MaxFallSpeed,
        CoyoteTime:             CoyoteTime,
        JumpBufferTime:         JumpBufferTime
    );
}
