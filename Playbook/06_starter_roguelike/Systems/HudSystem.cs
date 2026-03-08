using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Draws the heads-up display: HP bar, level, dungeon depth, and message log.
/// Requires a <see cref="SpriteFont"/> to render text.
/// </summary>
public sealed class HudSystem
{
    private static readonly QueryDescription PlayerQuery =
        new QueryDescription().WithAll<Stats, PlayerTag>();

    private Texture2D? _pixel;
    private SpriteFont? _font;

    // Layout
    private const int HudX = 8;
    private const int HudY = 4;
    private const int BarWidth = 200;
    private const int BarHeight = 16;
    private const int LineSpacing = 18;

    /// <summary>Initialize with graphics resources.</summary>
    public void Initialize(GraphicsDevice graphicsDevice, SpriteFont font)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _font = font;
    }

    /// <summary>
    /// Draw the HUD overlay. Call after map/entity rendering.
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, World world, MessageLog log, int dungeonDepth)
    {
        if (_pixel == null || _font == null) return;

        int y = HudY;

        // Find player stats
        Stats playerStats = default;
        world.Query(in PlayerQuery, (ref Stats stats) =>
        {
            playerStats = stats;
        });

        // HP bar background
        spriteBatch.Draw(_pixel,
            new Rectangle(HudX, y, BarWidth, BarHeight),
            new Color(60, 0, 0));

        // HP bar fill
        float hpRatio = playerStats.MaxHp > 0
            ? (float)playerStats.Hp / playerStats.MaxHp
            : 0f;
        spriteBatch.Draw(_pixel,
            new Rectangle(HudX, y, (int)(BarWidth * hpRatio), BarHeight),
            Color.DarkRed);

        // HP text
        string hpText = $"HP: {playerStats.Hp}/{playerStats.MaxHp}";
        spriteBatch.DrawString(_font, hpText,
            new Vector2(HudX + 4, y + 1), Color.White);

        y += BarHeight + 4;

        // Level and EXP
        spriteBatch.DrawString(_font,
            $"Lv {playerStats.Level}  EXP: {playerStats.Exp}/{playerStats.ExpToNext}",
            new Vector2(HudX, y), Color.LightGray);

        y += LineSpacing;

        // Dungeon depth
        spriteBatch.DrawString(_font,
            $"Depth: {dungeonDepth}",
            new Vector2(HudX, y), Color.LightGray);

        // Message log at the bottom of the screen
        DrawMessageLog(spriteBatch, log);
    }

    private void DrawMessageLog(SpriteBatch spriteBatch, MessageLog log)
    {
        if (_font == null || _pixel == null) return;

        int screenHeight = spriteBatch.GraphicsDevice.Viewport.Height;
        int logY = screenHeight - (RoguelikeConfig.VisibleLogMessages * LineSpacing) - 8;

        // Semi-transparent background
        spriteBatch.Draw(_pixel,
            new Rectangle(0, logY - 4,
                spriteBatch.GraphicsDevice.Viewport.Width,
                RoguelikeConfig.VisibleLogMessages * LineSpacing + 8),
            new Color(0, 0, 0, 150));

        foreach (var entry in log.GetRecent(RoguelikeConfig.VisibleLogMessages))
        {
            spriteBatch.DrawString(_font, entry.Text,
                new Vector2(HudX, logY), entry.Color);
            logY += LineSpacing;
        }
    }

    /// <summary>Dispose resources.</summary>
    public void Dispose()
    {
        _pixel?.Dispose();
    }
}
