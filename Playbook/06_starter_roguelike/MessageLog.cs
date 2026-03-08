using Microsoft.Xna.Framework;

namespace MyGame.Roguelike;

/// <summary>
/// Simple scrolling message log for combat feedback and game events.
/// "You hit the goblin for 3 damage!", "The goblin dies!", etc.
/// </summary>
public sealed class MessageLog
{
    private readonly List<LogEntry> _messages = new();

    /// <summary>All messages in the log, oldest first.</summary>
    public IReadOnlyList<LogEntry> Messages => _messages;

    /// <summary>A single log entry with text and color.</summary>
    public readonly record struct LogEntry(string Text, Color Color);

    /// <summary>Add a white message to the log.</summary>
    public void Add(string text) => Add(text, Color.White);

    /// <summary>Add a colored message to the log.</summary>
    public void Add(string text, Color color)
    {
        _messages.Add(new LogEntry(text, color));

        // Trim old messages
        while (_messages.Count > RoguelikeConfig.MaxLogMessages)
            _messages.RemoveAt(0);
    }

    /// <summary>Get the most recent N messages for display.</summary>
    public IEnumerable<LogEntry> GetRecent(int count)
    {
        int start = Math.Max(0, _messages.Count - count);
        for (int i = start; i < _messages.Count; i++)
            yield return _messages[i];
    }

    /// <summary>Clear all messages.</summary>
    public void Clear() => _messages.Clear();
}
