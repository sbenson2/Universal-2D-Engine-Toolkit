namespace MyGame.TopDown.Tags;

/// <summary>
/// Tag component marking an entity as a solid collider.
/// The collision system tests moving entities against all SolidTag entities
/// to prevent overlap (walls, trees, rocks, building boundaries).
/// </summary>
public record struct SolidTag;
