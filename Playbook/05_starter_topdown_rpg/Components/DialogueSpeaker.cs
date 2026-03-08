namespace MyGame.TopDown.Components;

/// <summary>
/// Marks an entity (typically an NPC) as a dialogue speaker.
/// Stores the speaker's display name, optional portrait key, and
/// a reference to their dialogue data ID (looked up in dialogue assets).
/// </summary>
/// <param name="Name">Display name shown in the dialogue box.</param>
/// <param name="PortraitKey">Asset key for the speaker's portrait texture (empty for none).</param>
/// <param name="DialogueId">ID referencing a <see cref="MyGame.TopDown.Dialogue.DialogueData"/> entry.</param>
public record struct DialogueSpeaker(string Name, string PortraitKey, string DialogueId);
