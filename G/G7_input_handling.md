# G7 — Input Handling
> **Category:** Guide · **Related:** [R1 Library Stack](../R/R1_library_stack.md) · [C1 Genre Reference](../C/C1_genre_reference.md)

---

## Apos.Input (Primary)

**Install:** `dotnet add package Apos.Input --version 2.5.0`

Provides JustPressed/JustReleased tracking for keyboard, mouse, and gamepad. Wraps MonoGame's raw input into a polling model with edge detection.

### Key Features
- `JustPressed` / `JustReleased` for all input types
- Keyboard, mouse, and gamepad support
- Clean polling API (no event callbacks needed)
- Touch support for mobile

---

## Input Buffering (Custom — Fighting/Platformer)

For frame-precise input reading (fighting games, tight platformers), implement a ring buffer:

```csharp
public class InputBuffer
{
    private readonly InputFrame[] _buffer;
    private int _head;
    
    public InputBuffer(int size = 10)
    {
        _buffer = new InputFrame[size];
    }
    
    public void Record(InputFrame frame)
    {
        _buffer[_head % _buffer.Length] = frame;
        _head++;
    }
    
    public bool WasPressed(Button button, int withinFrames)
    {
        for (int i = 0; i < withinFrames && i < _buffer.Length; i++)
        {
            int idx = (_head - 1 - i + _buffer.Length * 2) % _buffer.Length;
            if (_buffer[idx].HasFlag(button)) return true;
        }
        return false;
    }
}
```

**Use cases:** Coyote time (allow jump for N frames after leaving a ledge), input buffering (queue a jump press during landing animation), combo detection (fighting game input sequences).

---

## Abstraction Layer

For cross-platform (keyboard + gamepad + touch), create an input abstraction:

```csharp
public interface IInputProvider
{
    Vector2 MoveDirection { get; }
    bool JumpPressed { get; }
    bool AttackPressed { get; }
    // ...
}
```

Implement separately for keyboard, gamepad, and touch. Game systems query `IInputProvider` instead of raw hardware state.
