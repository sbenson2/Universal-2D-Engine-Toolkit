namespace MyGame.TopDown.Components;

/// <summary>
/// Axis-aligned collision body for a character or solid object.
/// Width and Height define the ground-plane footprint (typically smaller than the visual sprite).
/// In 3/4 top-down view, this represents the feet/base area — not the full sprite.
/// </summary>
/// <param name="Width">Footprint width in pixels.</param>
/// <param name="Height">Footprint height in pixels.</param>
public record struct CharacterBody(float Width, float Height);
