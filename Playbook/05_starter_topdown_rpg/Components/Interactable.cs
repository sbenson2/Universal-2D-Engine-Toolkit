namespace MyGame.TopDown.Components;

/// <summary>
/// Marks an entity as interactable by the player.
/// The interaction system checks proximity against <see cref="Radius"/>
/// and triggers behavior based on <see cref="ActionType"/>.
/// </summary>
/// <param name="Radius">Interaction radius in pixels from entity center.</param>
/// <param name="ActionType">
/// What happens on interact. Common values:
/// "dialogue" — opens the speaker's dialogue,
/// "pickup" — adds item to inventory,
/// "examine" — shows a text popup,
/// "transition" — triggers scene/area change.
/// </param>
public record struct Interactable(float Radius, string ActionType);
