using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.ECS.Components;

/// <summary>
/// Basic sprite component for 2D rendering.
/// </summary>
/// <param name="Texture">The texture to draw.</param>
/// <param name="SourceRect">Optional source rectangle for sprite sheets. Null draws the full texture.</param>
/// <param name="Color">Tint color. Defaults to White (no tint).</param>
/// <param name="Scale">Uniform scale factor. Defaults to 1.</param>
/// <param name="Rotation">Rotation in radians. Defaults to 0.</param>
/// <param name="LayerDepth">Draw order (0 = front, 1 = back). Defaults to 0.</param>
public record struct Sprite(
    Texture2D Texture,
    Rectangle? SourceRect = null,
    Color? Color = null,
    float Scale = 1f,
    float Rotation = 0f,
    float LayerDepth = 0f)
{
    /// <summary>Resolved tint color (defaults to White).</summary>
    public readonly Color ResolvedColor => Color ?? Microsoft.Xna.Framework.Color.White;
}
