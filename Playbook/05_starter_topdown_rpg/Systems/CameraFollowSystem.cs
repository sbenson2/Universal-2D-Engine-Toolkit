using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.TopDown.Tags;

namespace MyGame.TopDown.Systems;

/// <summary>
/// Smooth-follow camera that tracks the player entity.
/// Uses linear interpolation (lerp) for smoothing with optional world-bounds clamping.
/// The camera position represents the center of the viewport.
/// </summary>
/// <remarks>
/// Per G28 best practices:
/// - Bounds clamping happens AFTER smoothing to prevent jitter at edges.
/// - For pixel-perfect rendering, snap camera to whole pixels before drawing.
/// - Consider a dead zone for small movements (not included here — easy to add).
/// </remarks>
public static class CameraFollowSystem
{
    /// <summary>Current camera world position (center of viewport).</summary>
    public static Vector2 Position { get; private set; }

    /// <summary>Smoothing factor (0 = no movement, 1 = instant snap). Typical: 0.08–0.15.</summary>
    public static float SmoothSpeed { get; set; } = TopDownConfig.CameraSmoothSpeed;

    /// <summary>Optional world bounds for clamping. Null = no clamping.</summary>
    public static Rectangle? WorldBounds { get; set; }

    /// <summary>Viewport width in native resolution pixels.</summary>
    public static int ViewportWidth { get; set; } = TopDownConfig.NativeWidth;

    /// <summary>Viewport height in native resolution pixels.</summary>
    public static int ViewportHeight { get; set; } = TopDownConfig.NativeHeight;

    private static readonly QueryDescription PlayerQuery = new QueryDescription()
        .WithAll<Position, PlayerTag>();

    /// <summary>
    /// Register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        Vector2 target = Position;

        world.Query(in PlayerQuery, (ref Position pos) =>
        {
            target = new Vector2(pos.X, pos.Y);
        });

        // Smooth follow via lerp.
        Position = Vector2.Lerp(Position, target, SmoothSpeed);

        // Clamp to world bounds (after smoothing to prevent edge jitter).
        if (WorldBounds is { } bounds)
        {
            float halfW = ViewportWidth * 0.5f;
            float halfH = ViewportHeight * 0.5f;

            float minX = bounds.Left + halfW;
            float maxX = bounds.Right - halfW;
            float minY = bounds.Top + halfH;
            float maxY = bounds.Bottom - halfH;

            Position = new Vector2(
                MathHelper.Clamp(Position.X, minX, maxX),
                MathHelper.Clamp(Position.Y, minY, maxY)
            );
        }

        // Snap to pixel for pixel-perfect rendering.
        Position = new Vector2(
            MathF.Round(Position.X),
            MathF.Round(Position.Y)
        );
    }

    /// <summary>
    /// Builds the camera transform matrix for SpriteBatch.Begin.
    /// Translates world coordinates so the camera position is centered on screen.
    /// </summary>
    public static Matrix GetTransformMatrix()
    {
        return Matrix.CreateTranslation(
            -Position.X + ViewportWidth * 0.5f,
            -Position.Y + ViewportHeight * 0.5f,
            0f
        );
    }
}
