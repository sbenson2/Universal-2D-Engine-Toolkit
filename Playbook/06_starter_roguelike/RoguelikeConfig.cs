namespace MyGame.Roguelike;

/// <summary>
/// Central tuning constants for the roguelike. Adjust these to change game feel
/// without touching system logic.
/// </summary>
public static class RoguelikeConfig
{
    // ── Map ──────────────────────────────────────────────
    /// <summary>Map width in tiles.</summary>
    public const int MapWidth = 80;
    /// <summary>Map height in tiles.</summary>
    public const int MapHeight = 45;
    /// <summary>Maximum rooms the generator will attempt to place.</summary>
    public const int MaxRooms = 15;
    /// <summary>Minimum room dimension (width or height).</summary>
    public const int RoomMinSize = 5;
    /// <summary>Maximum room dimension (width or height).</summary>
    public const int RoomMaxSize = 12;

    // ── Rendering ────────────────────────────────────────
    /// <summary>Tile size in pixels for rendering.</summary>
    public const int TileSize = 16;

    // ── FOV ──────────────────────────────────────────────
    /// <summary>Player vision radius in tiles.</summary>
    public const int PlayerFovRadius = 8;

    // ── Turn System ──────────────────────────────────────
    /// <summary>Energy threshold required to take an action.</summary>
    public const int EnergyThreshold = 100;
    /// <summary>Default energy gained per tick for speed-10 actors.</summary>
    public const int BaseEnergyPerTurn = 10;

    // ── Player Defaults ──────────────────────────────────
    public const int PlayerMaxHp = 30;
    public const int PlayerAttack = 5;
    public const int PlayerDefense = 2;
    public const int PlayerSpeed = 10;
    public const int PlayerExpToNext = 20;
    public const int PlayerInventorySlots = 10;

    // ── Enemy Spawning ───────────────────────────────────
    /// <summary>Min enemies spawned per dungeon floor.</summary>
    public const int MinEnemiesPerFloor = 3;
    /// <summary>Max enemies spawned per dungeon floor.</summary>
    public const int MaxEnemiesPerFloor = 8;
    /// <summary>Min items spawned per dungeon floor.</summary>
    public const int MinItemsPerFloor = 1;
    /// <summary>Max items spawned per dungeon floor.</summary>
    public const int MaxItemsPerFloor = 4;

    // ── Enemy Stats ──────────────────────────────────────
    public const int GoblinMaxHp = 10;
    public const int GoblinAttack = 3;
    public const int GoblinDefense = 0;
    public const int GoblinSpeed = 8;
    public const int GoblinExpValue = 10;

    public const int OrcMaxHp = 20;
    public const int OrcAttack = 5;
    public const int OrcDefense = 1;
    public const int OrcSpeed = 6;
    public const int OrcExpValue = 25;

    // ── Combat ───────────────────────────────────────────
    /// <summary>HP threshold ratio below which AI flees (0.0–1.0).</summary>
    public const float AiFleeThreshold = 0.25f;

    /// <summary>
    /// Calculate damage dealt. Minimum 1 so attacks always do something.
    /// </summary>
    public static int CalculateDamage(int attack, int defense) =>
        Math.Max(1, attack - defense);

    // ── Leveling ─────────────────────────────────────────
    /// <summary>EXP required for next level, scaling by current level.</summary>
    public static int ExpForLevel(int level) => 20 + (level - 1) * 10;

    // ── Message Log ──────────────────────────────────────
    /// <summary>Maximum messages kept in the scrolling log.</summary>
    public const int MaxLogMessages = 50;
    /// <summary>Messages shown on-screen in the HUD.</summary>
    public const int VisibleLogMessages = 5;
}
