using Arch.Core;
using Apos.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyGame.Platformer.Components;
using MyGame.Platformer.Tags;

namespace MyGame.Platformer.Systems;

/// <summary>
/// Reads raw input via Apos.Input and writes a clean <see cref="PlayerIntent"/>
/// component each frame. All other systems read intent — never raw input.
/// This keeps input handling in one place and makes replays/AI trivial.
/// </summary>
public static class InputSystem
{
    // ── Apos.Input conditions ──
    // These are evaluated once per frame and cached by Apos.Input.
    private static readonly ICondition MoveLeft = new AnyCondition(
        new KeyboardCondition(Keys.A),
        new KeyboardCondition(Keys.Left),
        new GamePadCondition(GamePadButton.LeftThumbstickLeft, 0)
    );

    private static readonly ICondition MoveRight = new AnyCondition(
        new KeyboardCondition(Keys.D),
        new KeyboardCondition(Keys.Right),
        new GamePadCondition(GamePadButton.LeftThumbstickRight, 0)
    );

    private static readonly ICondition JumpPress = new AnyCondition(
        new KeyboardCondition(Keys.Space),
        new KeyboardCondition(Keys.W),
        new KeyboardCondition(Keys.Up),
        new GamePadCondition(GamePadButton.A, 0)
    );

    private static readonly ICondition JumpHold = new AnyCondition(
        new KeyboardCondition(Keys.Space, KeyboardConditionState.Down),
        new KeyboardCondition(Keys.W, KeyboardConditionState.Down),
        new KeyboardCondition(Keys.Up, KeyboardConditionState.Down),
        new GamePadCondition(GamePadButton.A, 0, GamePadConditionState.Down)
    );

    private static readonly ICondition DropDown = new AnyCondition(
        new KeyboardCondition(Keys.S, KeyboardConditionState.Down),
        new KeyboardCondition(Keys.Down, KeyboardConditionState.Down),
        new GamePadCondition(GamePadButton.LeftThumbstickDown, 0, GamePadConditionState.Down)
    );

    private static readonly QueryDescription PlayerQuery = new QueryDescription()
        .WithAll<PlayerIntent, PlayerTag>();

    /// <summary>
    /// Update method — register with <see cref="MyGame.ECS.WorldManager.AddUpdateSystem"/>.
    /// </summary>
    public static void Update(World world, GameTime gameTime)
    {
        // Build the intent for this frame.
        float moveX = 0f;
        if (MoveLeft.Pressed()) moveX -= 1f;
        if (MoveRight.Pressed()) moveX += 1f;

        bool jumpPressed = JumpPress.Pressed();
        bool jumpHeld = JumpHold.Pressed();
        bool dropDown = DropDown.Pressed() && jumpPressed;

        var intent = new PlayerIntent(moveX, jumpPressed, jumpHeld, dropDown);

        // Write to every entity tagged as the player.
        world.Query(in PlayerQuery, (ref PlayerIntent pi) =>
        {
            pi = intent;
        });
    }
}
