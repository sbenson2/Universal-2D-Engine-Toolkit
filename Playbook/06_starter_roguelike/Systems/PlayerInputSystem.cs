using Apos.Input;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Map;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Reads grid-based movement input (WASD / arrows) during the player's turn.
/// Also handles wait (period/numpad5) and interact (E key).
/// Uses Apos.Input for clean input handling.
/// </summary>
public sealed class PlayerInputSystem
{
    private static readonly QueryDescription PlayerQuery =
        new QueryDescription().WithAll<GridPosition, TurnActor, PlayerTag>();

    // Apos.Input conditions for movement
    private readonly ICondition _moveUp = new AnyCondition(
        new KeyboardCondition(Keys.W),
        new KeyboardCondition(Keys.Up));
    private readonly ICondition _moveDown = new AnyCondition(
        new KeyboardCondition(Keys.S),
        new KeyboardCondition(Keys.Down));
    private readonly ICondition _moveLeft = new AnyCondition(
        new KeyboardCondition(Keys.A),
        new KeyboardCondition(Keys.Left));
    private readonly ICondition _moveRight = new AnyCondition(
        new KeyboardCondition(Keys.D),
        new KeyboardCondition(Keys.Right));
    private readonly ICondition _wait = new AnyCondition(
        new KeyboardCondition(Keys.OemPeriod),
        new KeyboardCondition(Keys.NumPad5));
    private readonly ICondition _interact = new KeyboardCondition(Keys.E);

    /// <summary>Pending movement delta from input, or null if no input.</summary>
    public (int DX, int DY)? PendingMove { get; private set; }

    /// <summary>True if the player chose to wait this turn.</summary>
    public bool DidWait { get; private set; }

    /// <summary>True if the player pressed interact.</summary>
    public bool DidInteract { get; private set; }

    /// <summary>
    /// Poll input. Only processes when the turn system is waiting for the player.
    /// </summary>
    public void Update(World world, GameTime gameTime, TurnSystem turnSystem)
    {
        PendingMove = null;
        DidWait = false;
        DidInteract = false;

        if (!turnSystem.WaitingForInput || !turnSystem.IsPlayerTurn) return;

        if (_moveUp.Pressed())
            PendingMove = (0, -1);
        else if (_moveDown.Pressed())
            PendingMove = (0, 1);
        else if (_moveLeft.Pressed())
            PendingMove = (-1, 0);
        else if (_moveRight.Pressed())
            PendingMove = (1, 0);
        else if (_wait.Pressed())
            DidWait = true;
        else if (_interact.Pressed())
            DidInteract = true;
    }
}
