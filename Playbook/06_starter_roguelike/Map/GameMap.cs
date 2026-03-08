namespace MyGame.Roguelike.Map;

/// <summary>
/// 2D tile-based dungeon map. Stores tile types and fog-of-war state.
/// See G53_procedural_generation.md and G54_fog_of_war.md for the patterns used here.
/// </summary>
public sealed class GameMap
{
    /// <summary>Map width in tiles.</summary>
    public int Width { get; }

    /// <summary>Map height in tiles.</summary>
    public int Height { get; }

    private readonly TileType[,] _tiles;

    /// <summary>True if the tile has been seen at least once (persists across FOV updates).</summary>
    public bool[,] Explored { get; }

    /// <summary>True if the tile is currently visible to the player this turn.</summary>
    public bool[,] Visible { get; }

    /// <summary>
    /// Create a new map filled entirely with walls.
    /// </summary>
    public GameMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new TileType[width, height];
        Explored = new bool[width, height];
        Visible = new bool[width, height];

        // Default all tiles to wall
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _tiles[x, y] = TileType.Wall;
    }

    /// <summary>Returns true if (x, y) is inside the map bounds.</summary>
    public bool IsInBounds(int x, int y) =>
        x >= 0 && x < Width && y >= 0 && y < Height;

    /// <summary>Returns true if the tile is walkable (floor, stairs, or open door).</summary>
    public bool IsWalkable(int x, int y)
    {
        if (!IsInBounds(x, y)) return false;
        var tile = _tiles[x, y];
        return tile == TileType.Floor || tile == TileType.StairsDown || tile == TileType.Door;
    }

    /// <summary>Returns true if the tile blocks line of sight.</summary>
    public bool IsOpaque(int x, int y)
    {
        if (!IsInBounds(x, y)) return true;
        return _tiles[x, y] == TileType.Wall;
    }

    /// <summary>Get the tile type at (x, y). Returns Wall if out of bounds.</summary>
    public TileType GetTileAt(int x, int y)
    {
        if (!IsInBounds(x, y)) return TileType.Wall;
        return _tiles[x, y];
    }

    /// <summary>Set the tile type at (x, y). No-op if out of bounds.</summary>
    public void SetTile(int x, int y, TileType tile)
    {
        if (!IsInBounds(x, y)) return;
        _tiles[x, y] = tile;
    }

    /// <summary>
    /// Clear current-frame visibility. Called before recalculating FOV each turn.
    /// </summary>
    public void ClearVisible()
    {
        Array.Clear(Visible, 0, Visible.Length);
    }

    /// <summary>
    /// Mark a tile as visible (and explored). Called by the FOV algorithm.
    /// </summary>
    public void Reveal(int x, int y)
    {
        if (!IsInBounds(x, y)) return;
        Visible[x, y] = true;
        Explored[x, y] = true;
    }
}
