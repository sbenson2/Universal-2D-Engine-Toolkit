using Arch.Core;
using Apos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyGame.ECS.Components;
using MyGame.TopDown.Components;
using MyGame.TopDown.Tags;

namespace MyGame.TopDown.Systems;

/// <summary>
/// Reads keyboard input via Apos.Input and writes a normalized direction
/// into the player's <see cref="Velocity"/> component.
/// Supports 4-directional and 8-directional movement with diagonal normalization.
/// </summary>
/// <remarks>
/// This system only writes directional intent — actual acceleration/friction
/// is handled by <see cref="TopDownMovementSystem"/>.
/// </remarks>
public static class InputSystem
{
    // --- Apos.Input condition keys ---
    private static readonly ICondition MoveUp    = new AnyCondition(
        new KeyboardCondition(Keys.W),
        new KeyboardCondition(Keys.Up));

    private static readonly ICondition MoveDown  = new AnyCondition(
        new KeyboardCondition(Keys.S),
        new KeyboardCondition(Keys.Down));

    private static readonly ICondition MoveLeft  = new AnyCondition(
        new KeyboardCondition(Keys.A),
        new KeyboardCondition(Keys.Left));

    private static readonly ICondition MoveRight = new AnyCondition(
        new KeyboardCondition(Keys.D),
        new KeyboardCondition(Keys.Right));

    /// <summary>Condition for the interact button (E or Space).</summary>
    public static readonly ICondition Interact = new AnyCondition(
        new KeyboardCondition(Keys.E),
        new KeyboardCondition(Keys.Space));

    private static readonly QueryDescription PlayerQuery = new QueryDescription()
        .WithAll<Position, Velocity, CharacterMotion, FacingDirection, PlayerTag>();

    /// <summary>
    /// Register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// Reads directional input and sets the player's velocity to the desired direction
    /// scaled by move speed. Diagonal input is normalized to prevent faster diagonal movement.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        float dirX = 0f;
        float dirY = 0f;

        if (MoveLeft.Held())  dirX -= 1f;
        if (MoveRight.Held()) dirX += 1f;
        if (MoveUp.Held())    dirY -= 1f;
        if (MoveDown.Held())  dirY += 1f;

        // Normalize diagonal so you don't move ~1.41x faster.
        float len = MathF.Sqrt(dirX * dirX + dirY * dirY);
        if (len > 0f)
        {
            dirX /= len;
            dirY /= len;
        }

        world.Query(in PlayerQuery, (ref Velocity vel, ref CharacterMotion motion, ref FacingDirection facing) =>
        {
            // Store desired direction as velocity; TopDownMovementSystem applies accel/friction.
            vel = new Velocity(dirX * motion.MoveSpeed, dirY * motion.MoveSpeed);

            // Update facing only when there's input — keep last facing when idle.
            if (dirX != 0f || dirY != 0f)
            {
                facing = new FacingDirection(
                    dirX < 0f ? -1 : dirX > 0f ? 1 : 0,
                    dirY < 0f ? -1 : dirY > 0f ? 1 : 0
                );
            }
        });
    }
}
