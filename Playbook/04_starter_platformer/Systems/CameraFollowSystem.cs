using Arch.Core;
using Microsoft.Xna.Framework;
using MyGame.ECS.Components;
using MyGame.Platformer.Components;
using MyGame.Platformer.Tags;

namespace MyGame.Platformer.Systems;

/// <summary>
/// Smooth camera follow with deadzone and horizontal lookahead.
/// The camera only moves when the player exits the deadzone rectangle,
/// and shifts slightly ahead in the direction the player is facing.
/// </summary>
/// <remarks>
/// The camera position is stored as a static — in a real project you'd put this
/// on a dedicated Camera entity or behind a service. Kept simple here for the starter kit.
/// </remarks>
public static class CameraFollowSystem
{
    /// <summary>Current camera center position in world space.</summary>
    public static Vector2 CameraPosition;

    /// <summary>Half-width of the deadzone rectangle.</summary>
    private static readonly float DeadzoneHalfW = PlatformerConfig.CameraDeadzoneWidth * 0.5f;

    /// <summary>Half-height of the deadzone rectangle.</summary>
    private static readonly float DeadzoneHalfH = PlatformerConfig.CameraDeadzoneHeight * 0.5f;

    private static readonly QueryDescription Query = new QueryDescription()
        .WithAll<Position, FacingDirection, PlayerTag>();

    /// <summary>
    /// Update method — register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        world.Query(in Query, (ref Position pos, ref FacingDirection facing) =>
        {
            // Target = player position + lookahead in facing direction.
            float targetX = pos.X + facing.Direction * PlatformerConfig.CameraLookahead;
            float targetY = pos.Y;

            // Only move the camera when the player is outside the deadzone.
            float dx = targetX - CameraPosition.X;
            float dy = targetY - CameraPosition.Y;

            if (MathF.Abs(dx) > DeadzoneHalfW)
            {
                float edge = MathF.Sign(dx) * DeadzoneHalfW;
                float desired = targetX - edge;
                CameraPosition.X = Lerp(CameraPosition.X, desired, PlatformerConfig.CameraSmoothSpeed * dt);
            }

            if (MathF.Abs(dy) > DeadzoneHalfH)
            {
                float edge = MathF.Sign(dy) * DeadzoneHalfH;
                float desired = targetY - edge;
                CameraPosition.Y = Lerp(CameraPosition.Y, desired, PlatformerConfig.CameraSmoothSpeed * dt);
            }
        });
    }

    private static float Lerp(float a, float b, float t)
    {
        t = MathHelper.Clamp(t, 0f, 1f);
        return a + (b - a) * t;
    }

    /// <summary>
    /// Returns a view matrix that centers the camera. Pass to SpriteBatch.Begin.
    /// </summary>
    /// <param name="screenWidth">Viewport width in pixels.</param>
    /// <param name="screenHeight">Viewport height in pixels.</param>
    public static Matrix GetViewMatrix(int screenWidth, int screenHeight)
    {
        return Matrix.CreateTranslation(
            -CameraPosition.X + screenWidth * 0.5f,
            -CameraPosition.Y + screenHeight * 0.5f,
            0f);
    }
}
