using Microsoft.Xna.Framework;

namespace MyGame.Roguelike.Map;

/// <summary>
/// Room-and-corridor dungeon generator. Carves rooms into a wall-filled map
/// and connects them with L-shaped corridors. Returns spawn point and stair location.
/// See G53_procedural_generation.md §3 for the BSP approach this is based on.
/// </summary>
public sealed class DungeonGenerator
{
    private readonly Random _rng;

    /// <summary>Create a generator with the given seed for deterministic output.</summary>
    public DungeonGenerator(int seed)
    {
        _rng = new Random(seed);
    }

    /// <summary>
    /// Result of dungeon generation: the map, player spawn point, stairs location, and room list.
    /// </summary>
    public readonly record struct DungeonResult(
        GameMap Map,
        Point PlayerSpawn,
        Point StairsPosition,
        List<Rectangle> Rooms);

    /// <summary>
    /// Generate a dungeon with the given parameters.
    /// </summary>
    /// <param name="width">Map width in tiles.</param>
    /// <param name="height">Map height in tiles.</param>
    /// <param name="maxRooms">Maximum number of rooms to attempt.</param>
    /// <param name="roomMinSize">Minimum room dimension (width or height).</param>
    /// <param name="roomMaxSize">Maximum room dimension (width or height).</param>
    /// <returns>A <see cref="DungeonResult"/> with the generated dungeon.</returns>
    public DungeonResult Generate(int width, int height, int maxRooms, int roomMinSize, int roomMaxSize)
    {
        var map = new GameMap(width, height);
        var rooms = new List<Rectangle>();

        for (int i = 0; i < maxRooms; i++)
        {
            int w = _rng.Next(roomMinSize, roomMaxSize + 1);
            int h = _rng.Next(roomMinSize, roomMaxSize + 1);
            int x = _rng.Next(1, width - w - 1);
            int y = _rng.Next(1, height - h - 1);

            var newRoom = new Rectangle(x, y, w, h);

            // Check for overlap with existing rooms (with 1-tile padding)
            bool overlaps = false;
            foreach (var existing in rooms)
            {
                var padded = new Rectangle(
                    existing.X - 1, existing.Y - 1,
                    existing.Width + 2, existing.Height + 2);
                if (padded.Intersects(newRoom))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps) continue;

            CarveRoom(map, newRoom);

            if (rooms.Count > 0)
            {
                var prevCenter = rooms[^1].Center;
                var newCenter = newRoom.Center;
                ConnectRooms(map, prevCenter, newCenter);
            }

            rooms.Add(newRoom);
        }

        // Player spawns in the center of the first room
        var spawn = rooms.Count > 0
            ? rooms[0].Center
            : new Point(width / 2, height / 2);

        // Stairs in the center of the last room
        var stairs = rooms.Count > 1
            ? rooms[^1].Center
            : new Point(spawn.X + 5, spawn.Y + 5);

        map.SetTile(stairs.X, stairs.Y, TileType.StairsDown);

        return new DungeonResult(map, spawn, stairs, rooms);
    }

    private static void CarveRoom(GameMap map, Rectangle room)
    {
        for (int x = room.X; x < room.X + room.Width; x++)
            for (int y = room.Y; y < room.Y + room.Height; y++)
                map.SetTile(x, y, TileType.Floor);
    }

    private void ConnectRooms(GameMap map, Point from, Point to)
    {
        // L-shaped corridor: randomly choose horizontal-first or vertical-first
        if (_rng.Next(2) == 0)
        {
            CarveHorizontalTunnel(map, from.X, to.X, from.Y);
            CarveVerticalTunnel(map, from.Y, to.Y, to.X);
        }
        else
        {
            CarveVerticalTunnel(map, from.Y, to.Y, from.X);
            CarveHorizontalTunnel(map, from.X, to.X, to.Y);
        }
    }

    private static void CarveHorizontalTunnel(GameMap map, int x1, int x2, int y)
    {
        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        for (int x = minX; x <= maxX; x++)
            map.SetTile(x, y, TileType.Floor);
    }

    private static void CarveVerticalTunnel(GameMap map, int y1, int y2, int x)
    {
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);
        for (int y = minY; y <= maxY; y++)
            map.SetTile(x, y, TileType.Floor);
    }
}
