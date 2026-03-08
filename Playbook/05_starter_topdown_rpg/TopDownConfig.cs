namespace MyGame.TopDown;

/// <summary>
/// Central tuning constants for the top-down RPG starter kit.
/// Adjust these to change the game's feel without modifying system code.
/// </summary>
public static class TopDownConfig
{
    // ── Rendering ──────────────────────────────────────────────
    /// <summary>Native render width (scaled up to window). 480×270 = clean 4× to 1080p.</summary>
    public const int NativeWidth = 480;
    /// <summary>Native render height.</summary>
    public const int NativeHeight = 270;
    /// <summary>Tile size in pixels (standard for 3/4 top-down).</summary>
    public const int TileSize = 16;

    // ── Player Movement ────────────────────────────────────────
    /// <summary>Default player move speed in pixels/second.</summary>
    public const float DefaultMoveSpeed = 80f;
    /// <summary>Acceleration toward max speed (pixels/sec²).</summary>
    public const float DefaultAcceleration = 600f;
    /// <summary>Friction deceleration when no input (pixels/sec²).</summary>
    public const float DefaultFriction = 500f;

    // ── Collision ──────────────────────────────────────────────
    /// <summary>Default player collision body width (ground footprint).</summary>
    public const float PlayerBodyWidth = 10f;
    /// <summary>Default player collision body height (ground footprint).</summary>
    public const float PlayerBodyHeight = 6f;

    // ── Camera ─────────────────────────────────────────────────
    /// <summary>Camera lerp smoothing factor. 0.1 = smooth, 1.0 = instant.</summary>
    public const float CameraSmoothSpeed = 0.1f;

    // ── Interaction ────────────────────────────────────────────
    /// <summary>Default interaction radius for interactable entities (pixels).</summary>
    public const float DefaultInteractionRadius = 20f;
    /// <summary>Offset in the facing direction for interaction checks (pixels).</summary>
    public const float InteractionFacingOffset = 8f;

    // ── Dialogue ───────────────────────────────────────────────
    /// <summary>Characters revealed per second in typewriter effect.</summary>
    public const float DialogueCharsPerSecond = 30f;
    /// <summary>Dialogue box height in pixels (native resolution).</summary>
    public const int DialogueBoxHeight = 56;
    /// <summary>Padding around the dialogue box (pixels).</summary>
    public const int DialogueBoxPadding = 8;

    // ── Stats (starter values) ─────────────────────────────────
    /// <summary>Default player starting HP.</summary>
    public const int DefaultMaxHp = 30;
    /// <summary>Default player attack.</summary>
    public const int DefaultAttack = 5;
    /// <summary>Default player defense.</summary>
    public const int DefaultDefense = 3;
    /// <summary>Default player speed stat.</summary>
    public const int DefaultSpeed = 5;
}
