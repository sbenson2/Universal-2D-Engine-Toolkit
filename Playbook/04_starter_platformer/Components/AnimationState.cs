namespace MyGame.Platformer.Components;

/// <summary>
/// Current animation state for rendering. Updated each frame by
/// <see cref="MyGame.Platformer.Systems.AnimationStateSystem"/> based on
/// velocity, grounded state, and facing direction.
/// </summary>
/// <param name="CurrentAnim">Animation key: "idle", "run", "jump", or "fall".</param>
/// <param name="FlipX">Whether the sprite should be horizontally flipped (true = facing left).</param>
public record struct AnimationState(string CurrentAnim, bool FlipX);
