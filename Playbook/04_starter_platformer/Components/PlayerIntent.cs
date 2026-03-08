namespace MyGame.Platformer.Components;

/// <summary>
/// Buffered player input for the current frame, written by
/// <see cref="MyGame.Platformer.Systems.InputSystem"/> and consumed by movement/jump systems.
/// Decouples raw input reading from gameplay logic.
/// </summary>
/// <param name="MoveX">Horizontal input axis: -1 (left), 0 (none), or 1 (right).</param>
/// <param name="JumpPressed">True only on the frame the jump button was first pressed.</param>
/// <param name="JumpHeld">True every frame the jump button is held down.</param>
/// <param name="DropDown">True when the player wants to drop through a one-way platform (down + jump).</param>
public record struct PlayerIntent(float MoveX, bool JumpPressed, bool JumpHeld, bool DropDown);
