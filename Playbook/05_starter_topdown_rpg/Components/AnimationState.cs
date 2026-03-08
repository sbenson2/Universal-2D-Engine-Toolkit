namespace MyGame.TopDown.Components;

/// <summary>
/// Tracks the current animation name and horizontal flip state.
/// The animation system derives this from velocity + facing each frame.
/// Renderers use <see cref="CurrentAnim"/> to look up sprite sheet regions.
/// </summary>
/// <param name="CurrentAnim">Animation key, e.g. "idle_down", "walk_side", "walk_up".</param>
/// <param name="FlipX">If true, the sprite is mirrored horizontally (reuse left/right art).</param>
public record struct AnimationState(string CurrentAnim, bool FlipX);
