namespace MyGame.ECS.Tags;

/// <summary>
/// Empty tag component to identify the player entity.
/// Used for query filtering — attach to an entity to mark it as the player.
/// </summary>
public record struct PlayerTag;
