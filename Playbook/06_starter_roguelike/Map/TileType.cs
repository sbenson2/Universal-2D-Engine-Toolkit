namespace MyGame.Roguelike.Map;

/// <summary>
/// Types of dungeon tiles.
/// </summary>
public enum TileType : byte
{
    /// <summary>Open walkable floor.</summary>
    Floor = 0,
    /// <summary>Solid wall — blocks movement and sight.</summary>
    Wall = 1,
    /// <summary>Stairs leading to the next dungeon depth.</summary>
    StairsDown = 2,
    /// <summary>Door — can be opened to become walkable.</summary>
    Door = 3
}
