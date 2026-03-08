namespace MyGame.TopDown.Dialogue;

/// <summary>
/// Data structure for a dialogue tree. A dialogue consists of a sequence of <see cref="DialogueLine"/>
/// entries, each optionally offering <see cref="DialogueChoice"/> branches.
/// </summary>
public class DialogueData
{
    /// <summary>Unique ID for this dialogue (matches <see cref="Components.DialogueSpeaker.DialogueId"/>).</summary>
    public string Id { get; init; } = "";

    /// <summary>Ordered list of dialogue lines.</summary>
    public List<DialogueLine> Lines { get; init; } = new();
}

/// <summary>
/// A single line of dialogue, optionally with player choices.
/// </summary>
public class DialogueLine
{
    /// <summary>Speaker name override (empty = use the entity's DialogueSpeaker.Name).</summary>
    public string Speaker { get; init; } = "";

    /// <summary>The dialogue text displayed in the text box.</summary>
    public string Text { get; init; } = "";

    /// <summary>
    /// Optional choices presented to the player after this line.
    /// If empty, pressing advance goes to the next line in sequence.
    /// </summary>
    public List<DialogueChoice> Choices { get; init; } = new();
}

/// <summary>
/// A player-selectable choice within a dialogue line.
/// </summary>
public class DialogueChoice
{
    /// <summary>Display text for this choice.</summary>
    public string Label { get; init; } = "";

    /// <summary>
    /// Dialogue ID to jump to when selected (empty = continue to next line).
    /// Use this for branching dialogue trees.
    /// </summary>
    public string JumpToDialogueId { get; init; } = "";

    /// <summary>
    /// Line index within the target dialogue to jump to (0-based).
    /// Only used when <see cref="JumpToDialogueId"/> is set.
    /// </summary>
    public int JumpToLineIndex { get; init; }
}
