namespace MyGame.Platformer.Components;

/// <summary>
/// Kinematic character body defining the collision shape and grounded state.
/// Uses AABB (axis-aligned bounding box) collision — no physics engine needed.
/// </summary>
/// <param name="Width">Collision box width in pixels.</param>
/// <param name="Height">Collision box height in pixels.</param>
/// <param name="IsGrounded">Whether the character is currently standing on solid ground.</param>
/// <param name="WasGrounded">Grounded state from the previous frame (for edge detection).</param>
/// <param name="CoyoteTimer">Time remaining where a jump is still allowed after leaving ground.</param>
/// <param name="JumpBufferTimer">Time remaining for a buffered jump input to execute on landing.</param>
public record struct CharacterBody(
    float Width,
    float Height,
    bool IsGrounded,
    bool WasGrounded,
    float CoyoteTimer,
    float JumpBufferTimer);
