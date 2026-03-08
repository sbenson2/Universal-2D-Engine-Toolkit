# 04 — Platformer Starter Kit

> **Copy-paste-ready** 2D platformer on top of the [Project Template](../01_project_template/).  
> Kinematic character controller with coyote time, jump buffering, variable jump height, and smooth camera follow.  
> Targets **MonoGame.Framework.DesktopGL** + **Arch ECS v2.1.0** + **Apos.Input**.

---

## What's Inside

```
04_starter_platformer/
├── README.md                          ← you are here
├── PlatformerConfig.cs                ← all tuning knobs in one place
├── Components/
│   ├── CharacterBody.cs               ← collision shape + grounded state + timers
│   ├── CharacterMotion.cs             ← movement parameters (speed, accel, gravity…)
│   ├── FacingDirection.cs             ← 1 (right) or -1 (left)
│   ├── AnimationState.cs              ← current anim key + flip flag
│   └── PlayerIntent.cs               ← buffered input (written by InputSystem)
├── Tags/
│   ├── PlayerTag.cs                   ← marks the player entity
│   ├── GroundTag.cs                   ← marks solid ground tiles
│   └── OneWayPlatformTag.cs           ← marks drop-through platforms
├── Systems/
│   ├── InputSystem.cs                 ← reads Apos.Input → writes PlayerIntent
│   ├── CharacterMovementSystem.cs     ← horizontal accel/friction (ground vs air)
│   ├── GravitySystem.cs              ← gravity + fall multiplier + variable jump
│   ├── JumpSystem.cs                 ← coyote time + jump buffering + launch
│   ├── GroundDetectionSystem.cs      ← AABB ground check, coyote timer management
│   ├── AnimationStateSystem.cs       ← idle / run / jump / fall from velocity
│   └── CameraFollowSystem.cs         ← smooth follow with deadzone + lookahead
└── Scenes/
    └── PlatformerScene.cs             ← wires everything up, spawns player + level
```

---

## How to Use

### 1. Start from the Project Template

Copy `01_project_template/` as your project base. It gives you `GameApp`, `SceneManager`, `Scene`, `WorldManager`, `ServiceLocator`, and the shared `Position`/`Velocity` components.

### 2. Add NuGet Dependencies

If you haven't already:

```xml
<!-- In your .csproj -->
<PackageReference Include="Arch" Version="2.1.0" />
<PackageReference Include="Apos.Input" Version="4.*" />
```

### 3. Copy Platformer Files

Copy the entire `04_starter_platformer/` folder contents into your project's `src/` directory (or wherever you keep source files). The namespace is `MyGame.Platformer` — it sits alongside the base `MyGame` namespace from the template.

### 4. Wire Up the Scene

In your `GameApp` or wherever you push your first scene:

```csharp
using MyGame.Platformer.Scenes;

// Replace the default GameplayScene with PlatformerScene:
SceneManager.Push(new PlatformerScene());
```

### 5. Initialize Apos.Input

In your `GameApp.Initialize()` (or equivalent), add:

```csharp
Apos.Input.InputHelper.Setup(this);
```

And in `GameApp.Update()`, before `SceneManager.Update()`:

```csharp
Apos.Input.InputHelper.UpdateSetup();
// ... your update logic ...
Apos.Input.InputHelper.UpdateCleanup();
```

### 6. Run It

You'll see a debug-rendered player (colored rectangle) on a simple tile level. Move with **WASD/Arrows**, jump with **Space/W/Up**. The player changes color by state: white (idle), green (run), cyan (jump), orange (fall).

---

## Architecture

### System Execution Order

Systems run in this order each frame — order matters:

| # | System | Purpose |
|---|--------|---------|
| 1 | **InputSystem** | Read hardware input → write `PlayerIntent` |
| 2 | **CharacterMovementSystem** | Apply horizontal acceleration/friction |
| 3 | **GravitySystem** | Apply gravity (with fall multiplier + variable jump) |
| 4 | **JumpSystem** | Check coyote time + buffer, launch if conditions met |
| 5 | **GroundDetectionSystem** | AABB overlap test, set `IsGrounded`, manage coyote timer |
| 6 | **AnimationStateSystem** | Derive animation key from velocity + grounded |
| 7 | **CameraFollowSystem** | Smooth follow with deadzone |

### Component Layout

Every player entity has these components:

| Component | Source | Purpose |
|-----------|--------|---------|
| `Position` | Project template | World-space XY |
| `Velocity` | Project template | Per-frame velocity (Dx, Dy) |
| `CharacterBody` | This kit | Collision size + grounded state + timers |
| `CharacterMotion` | This kit | All movement tuning parameters |
| `PlayerIntent` | This kit | Buffered input snapshot |
| `FacingDirection` | This kit | Left/right for sprite flipping |
| `AnimationState` | This kit | Current animation key + flip |
| `PlayerTag` | This kit | Query filter tag |

Ground tiles have: `Position` + `GroundTag`.

### Philosophy

This kit follows the **G52 character controller philosophy**:

- **Kinematic > physics.** We set velocity directly — no `AddForce`, no rigid body. Every frame of movement is intentional.
- **Game feel first.** Jump height and time-to-apex are the primary design inputs. Gravity is *derived* from them.
- **Forgiveness windows.** Coyote time handles "jumped too late." Jump buffering handles "jumped too early." Together they make the game feel responsive without the player knowing why.
- **Variable jump height.** Release the button early = shorter jump. One button, full control.

---

## Tuning Guide

All constants live in **`PlatformerConfig.cs`**. Start here when tweaking feel.

### The Two Numbers That Matter Most

```csharp
public const float JumpHeight = 72f;    // How high (pixels)
public const float TimeToApex = 0.35f;  // How long to get there (seconds)
```

Everything else is derived or secondary. Change these first.

### Quick Presets

| Feel | JumpHeight | TimeToApex | FallMult | MoveSpeed | Accel |
|------|-----------|------------|----------|-----------|-------|
| **Tight** (Celeste) | 64 | 0.28 | 2.5 | 200 | 2800 |
| **Balanced** (default) | 72 | 0.35 | 2.0 | 200 | 1800 |
| **Floaty** (Ori) | 100 | 0.50 | 1.4 | 180 | 1200 |
| **Heavy** (Hollow Knight) | 80 | 0.40 | 2.0 | 160 | 2200 |

### Forgiveness Windows

```csharp
public const float CoyoteTime = 0.1f;      // ~6 frames — standard
public const float JumpBufferTime = 0.133f; // ~8 frames — generous
```

Increase for a more forgiving game. Decrease for hardcore precision.

---

## Customizing

### Replace Debug Rendering with Sprites

In `PlatformerScene.DrawPlayer()`, swap the colored rectangle for your sprite sheet:

```csharp
// Read AnimationState to pick the right frame:
ref var anim = ref world.Get<AnimationState>(playerEntity);
var effect = anim.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
spriteBatch.Draw(spriteSheet, position, sourceRect, Color.White, 0f, origin, 1f, effect, 0f);
```

### Add One-Way Platforms

Spawn tiles with `OneWayPlatformTag` instead of `GroundTag`, then modify `GroundDetectionSystem` to only collide when the player's velocity is downward and their feet were above the platform last frame. See G52 §10 for the full pattern.

### Add Enemies/NPCs

The `CharacterBody` + `CharacterMotion` components work for any kinematic entity. Skip `PlayerTag` and `PlayerIntent` — instead write a simple AI system that sets velocity directly. Same physics, different brain.

### Add a Tilemap

Replace the `BuildLevel()` method with your tilemap loader. Each solid tile becomes a `Position` + `GroundTag` entity. For large maps, add spatial hashing to `GroundDetectionSystem` so you only check nearby tiles.

---

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| MonoGame.Framework.DesktopGL | 3.8+ | Rendering, input, game loop |
| Arch | 2.1.0 | Entity Component System |
| Apos.Input | 4.x | Input abstraction (keyboard, gamepad) |

---

## Related

- **[01_project_template](../01_project_template/)** — base project this builds on
- **[G52 — Character Controller](../../G/G52_character_controller.md)** — full theory, wall mechanics, slopes, dashes, collision resolution
- **[G7 — Input Handling](../../G/G7_input_handling.md)** — input abstraction patterns
- **[G53 — Side-Scrolling Perspective](../../G/G53_side_scrolling.md)** — camera systems in depth
