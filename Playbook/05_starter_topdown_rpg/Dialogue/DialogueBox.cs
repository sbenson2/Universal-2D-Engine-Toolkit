using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.TopDown.Dialogue;

/// <summary>
/// Simple dialogue box with typewriter text reveal, speaker name display, and advance-on-button.
/// This is a UI-level class — not an ECS system. The <see cref="Systems.InteractionSystem"/>
/// triggers it, and the scene calls <see cref="Update"/> and <see cref="Draw"/> each frame.
/// </summary>
/// <remarks>
/// For production: extend with portraits, choice selection UI, and text formatting (color codes, etc.).
/// </remarks>
public class DialogueBox
{
    /// <summary>Whether the dialogue box is currently showing.</summary>
    public bool IsActive { get; private set; }

    private readonly Dictionary<string, DialogueData> _dialogueDatabase;
    private readonly SpriteFont _font;

    private DialogueData? _currentDialogue;
    private int _currentLineIndex;
    private string _currentSpeaker = "";
    private string _fullText = "";
    private string _displayedText = "";
    private float _charTimer;
    private int _charIndex;
    private bool _lineComplete;

    /// <summary>
    /// Creates a new DialogueBox.
    /// </summary>
    /// <param name="font">Font for rendering dialogue text.</param>
    /// <param name="dialogueDatabase">Map of dialogue ID → DialogueData.</param>
    public DialogueBox(SpriteFont font, Dictionary<string, DialogueData> dialogueDatabase)
    {
        _font = font;
        _dialogueDatabase = dialogueDatabase;
    }

    /// <summary>
    /// Start a dialogue sequence by ID.
    /// </summary>
    /// <param name="speakerName">Default speaker name (from DialogueSpeaker component).</param>
    /// <param name="dialogueId">Dialogue data ID to look up.</param>
    public void StartDialogue(string speakerName, string dialogueId)
    {
        if (!_dialogueDatabase.TryGetValue(dialogueId, out var data)) return;
        if (data.Lines.Count == 0) return;

        _currentDialogue = data;
        _currentLineIndex = 0;
        _currentSpeaker = speakerName;
        IsActive = true;

        BeginLine(data.Lines[0]);
    }

    /// <summary>
    /// Call every frame while the dialogue box is active.
    /// Handles typewriter progression and advance input.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!IsActive || _currentDialogue == null) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!_lineComplete)
        {
            // Typewriter effect: reveal one character at a time.
            _charTimer += dt;
            float charDelay = 1f / TopDownConfig.DialogueCharsPerSecond;

            while (_charTimer >= charDelay && _charIndex < _fullText.Length)
            {
                _charTimer -= charDelay;
                _charIndex++;
                _displayedText = _fullText[.._charIndex];
            }

            if (_charIndex >= _fullText.Length)
                _lineComplete = true;

            // Fast-complete on button press.
            if (Systems.InputSystem.Interact.Pressed() && !_lineComplete)
            {
                _displayedText = _fullText;
                _charIndex = _fullText.Length;
                _lineComplete = true;
                return; // Consume the press.
            }
        }
        else
        {
            // Line is fully revealed — advance on button press.
            if (Systems.InputSystem.Interact.Pressed())
            {
                AdvanceLine();
            }
        }
    }

    /// <summary>
    /// Draws the dialogue box. Call during the UI draw pass (no camera transform).
    /// </summary>
    /// <param name="spriteBatch">Active SpriteBatch (already in Begin/End block, or call your own).</param>
    /// <param name="screenWidth">Screen/viewport width for positioning.</param>
    /// <param name="screenHeight">Screen/viewport height for positioning.</param>
    public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        if (!IsActive) return;

        // --- Box background ---
        int boxHeight = TopDownConfig.DialogueBoxHeight;
        int padding = TopDownConfig.DialogueBoxPadding;
        var boxRect = new Rectangle(padding, screenHeight - boxHeight - padding,
                                     screenWidth - padding * 2, boxHeight);

        // Draw a semi-transparent black background.
        // (Requires a 1×1 white pixel texture — common pattern in MonoGame.)
        // For the starter kit, we'll just draw the text; add a texture for the box in production.

        // --- Speaker name ---
        var namePos = new Vector2(boxRect.X + 8, boxRect.Y + 4);
        spriteBatch.DrawString(_font, _currentSpeaker, namePos, Color.Yellow);

        // --- Dialogue text ---
        var textPos = new Vector2(boxRect.X + 8, boxRect.Y + 24);
        spriteBatch.DrawString(_font, _displayedText, textPos, Color.White);

        // --- Advance indicator ---
        if (_lineComplete)
        {
            var indicatorPos = new Vector2(boxRect.Right - 20, boxRect.Bottom - 16);
            spriteBatch.DrawString(_font, "▼", indicatorPos, Color.White);
        }
    }

    private void BeginLine(DialogueLine line)
    {
        _fullText = line.Text;
        _displayedText = "";
        _charTimer = 0f;
        _charIndex = 0;
        _lineComplete = false;

        if (!string.IsNullOrEmpty(line.Speaker))
            _currentSpeaker = line.Speaker;
    }

    private void AdvanceLine()
    {
        if (_currentDialogue == null) return;

        _currentLineIndex++;

        if (_currentLineIndex >= _currentDialogue.Lines.Count)
        {
            // End of dialogue.
            IsActive = false;
            _currentDialogue = null;
            return;
        }

        BeginLine(_currentDialogue.Lines[_currentLineIndex]);
    }
}
