# G17 — Testing
> **Category:** Guide · **Related:** [G16 Debugging](./G16_debugging.md) · [G11 Programming Principles](./G11_programming_principles.md) · [G12 Design Patterns](./G12_design_patterns.md)

---

## What to Test

**Worth it:** Damage calculations, state machine transitions, inventory logic, AI decision functions, pathfinding, serialization/deserialization, command execute/undo, event bus subscribe/raise.

**Not worth it:** Rendering output, "fun factor," emergent behavior, frame-perfect visual timing.

Focus testing effort on **deterministic systems** where bugs are reproducible and assertions meaningful.

---

## Unit Testing Pure Game Logic

```csharp
[Test]
public void TakeDamage_ReducesHealth()
{
    var health = new HealthComponent { MaxHealth = 100 };
    health.Initialize();

    health.TakeDamage(30);

    Assert.AreEqual(70, health.CurrentHealth);
}

[Test]
public void TakeDamage_EmitsDied_WhenHealthReachesZero()
{
    var health = new HealthComponent { MaxHealth = 100 };
    health.Initialize();
    bool died = false;
    health.Died += () => died = true;

    health.TakeDamage(100);

    Assert.IsTrue(died);
}
```

---

## Integration Testing via Interfaces

Mock engine services through interfaces so game logic can be tested in isolation:

```csharp
public interface IInputService
{
    Vector2 GetMovementVector();
    bool IsActionJustPressed(string action);
}

// Real implementation uses Apos.Input
public class RealInputService : IInputService { /* ... */ }

// Test implementation returns controlled values
public class MockInputService : IInputService
{
    public Vector2 MovementToReturn { get; set; }
    public HashSet<string> PressedActions { get; set; } = new();

    public Vector2 GetMovementVector() => MovementToReturn;
    public bool IsActionJustPressed(string action) => PressedActions.Contains(action);
}
```

This pattern works with the Service Locator described in [G12](./G12_design_patterns.md). During tests, provide mock services. During gameplay, provide real implementations.

---

## Testing Arch ECS Systems

Create a test World, add entities with known components, run a system once, assert the results:

```csharp
[Test]
public void MovementSystem_UpdatesPosition()
{
    var world = World.Create();
    var entity = world.Create(
        new Position { X = 0, Y = 0 },
        new Velocity { X = 100, Y = 0 }
    );

    var system = new MovementSystem(world);
    system.Update(1.0f / 60f); // One tick

    var pos = world.Get<Position>(entity);
    Assert.AreEqual(100f / 60f, pos.X, 0.001f);
}
```
