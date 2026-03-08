using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Map;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Renders the dungeon map and entities. Draws tiles as colored rectangles,
/// with fog-of-war (unexplored = black, explored = dark, visible = lit).
/// Entities are rendered as colored rectangles within visible tiles.
/// </summary>
public sealed class RenderSystem
{
    private static readonly QueryDescription RenderableQuery =
        new QueryDescription().WithAll<GridPosition>();

    private Texture2D? _pixel;
    private SpriteFont? _font;

    // Tile colors
    private static readonly Color FloorVisible = new(50, 50, 60);
    private static readonly Color FloorExplored = new(20, 20, 30);
    private static readonly Color WallVisible = new(100, 100, 120);
    private static readonly Color WallExplored = new(40, 40, 50);
    private static readonly Color StairsColor = Color.Gold;
    private static readonly Color DoorColor = new(139, 90, 43);
    private static readonly Color Unexplored = Color.Black;

    // Entity colors
    private static readonly Color PlayerColor = Color.Yellow;
    private static readonly Color EnemyColor = Color.Red;
    private static readonly Color ItemColor = Color.Cyan;

    /// <summary>
    /// Initialize rendering resources.
    /// </summary>
    public void Initialize(GraphicsDevice graphicsDevice, SpriteFont? font = null)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _font = font;
    }

    /// <summary>
    /// Draw the map and all visible entities.
    /// </summary>
    /// <param name="spriteBatch">Active SpriteBatch (Begin already called).</param>
    /// <param name="world">Arch ECS world.</param>
    /// <param name="map">The game map.</param>
    /// <param name="cameraOffset">Pixel offset for camera scrolling.</param>
    public void Draw(SpriteBatch spriteBatch, World world, GameMap map, Vector2 cameraOffset)
    {
        if (_pixel == null) return;

        int tileSize = RoguelikeConfig.TileSize;

        // Draw map tiles
        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                bool visible = map.Visible[x, y];
                bool explored = map.Explored[x, y];

                Color color;
                if (visible)
                {
                    color = map.GetTileAt(x, y) switch
                    {
                        TileType.Floor => FloorVisible,
                        TileType.Wall => WallVisible,
                        TileType.StairsDown => StairsColor,
                        TileType.Door => DoorColor,
                        _ => Unexplored
                    };
                }
                else if (explored)
                {
                    color = map.GetTileAt(x, y) switch
                    {
                        TileType.Floor => FloorExplored,
                        TileType.Wall => WallExplored,
                        TileType.StairsDown => new Color(100, 80, 0),
                        TileType.Door => new Color(70, 45, 20),
                        _ => Unexplored
                    };
                }
                else
                {
                    color = Unexplored;
                }

                var rect = new Rectangle(
                    (int)(x * tileSize + cameraOffset.X),
                    (int)(y * tileSize + cameraOffset.Y),
                    tileSize, tileSize);

                spriteBatch.Draw(_pixel, rect, color);
            }
        }

        // Draw entities (only in visible tiles)
        world.Query(in RenderableQuery, (Entity entity, ref GridPosition pos) =>
        {
            if (!map.Visible[pos.X, pos.Y]) return;

            Color entityColor;
            if (entity.Has<PlayerTag>())
                entityColor = PlayerColor;
            else if (entity.Has<EnemyTag>())
                entityColor = EnemyColor;
            else if (entity.Has<ItemTag>())
                entityColor = ItemColor;
            else
                entityColor = Color.Gray;

            // Draw entity as a slightly smaller rectangle within the tile
            int padding = 2;
            var rect = new Rectangle(
                (int)(pos.X * tileSize + cameraOffset.X) + padding,
                (int)(pos.Y * tileSize + cameraOffset.Y) + padding,
                tileSize - padding * 2,
                tileSize - padding * 2);

            spriteBatch.Draw(_pixel, rect, entityColor);
        });
    }

    /// <summary>Dispose the pixel texture.</summary>
    public void Dispose()
    {
        _pixel?.Dispose();
    }
}
