# 🎮 MyGame — Project Template

A copy-paste-ready starter kit for building 2D games with **MonoGame** + **Arch ECS**.

## What's Included

```
MyGame/
├── MyGame.csproj          # .NET 10, all dependencies pre-configured
├── Program.cs             # Minimal entry point
├── src/
│   ├── Core/
│   │   ├── GameApp.cs         # MonoGame Game subclass — graphics, scene pump
│   │   ├── SceneManager.cs    # Push/pop/switch scene stack
│   │   ├── ServiceLocator.cs  # Static service registry
│   │   └── Scene.cs           # Abstract base scene
│   ├── ECS/
│   │   ├── Components/
│   │   │   ├── Position.cs    # record struct Position(float X, float Y)
│   │   │   ├── Velocity.cs    # record struct Velocity(float Dx, float Dy)
│   │   │   └── Sprite.cs      # Basic sprite component
│   │   ├── Systems/
│   │   │   └── MovementSystem.cs  # Moves entities by velocity
│   │   ├── Tags/
│   │   │   └── PlayerTag.cs   # Empty tag struct
│   │   └── WorldManager.cs    # Arch World lifecycle + system registration
│   └── Scenes/
│       ├── MainMenuScene.cs   # Placeholder main menu
│       └── GameplayScene.cs   # Placeholder gameplay with ECS world
```

## Quick Start

1. **Copy this folder** and rename `MyGame` to your project name
2. Find-and-replace `MyGame` namespace with your own
3. Run:
   ```bash
   dotnet restore
   dotnet run
   ```

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| MonoGame.Framework.DesktopGL | 3.8.3.1 | Game framework (OpenGL) |
| Arch | 2.1.0 | Entity Component System |
| MonoGame.Extended | 5.3.1 | Cameras, collections, math helpers |
| Apos.Input | 2.5.0 | Clean input handling |
| FontStashSharp.MonoGame | 1.3.7 | Runtime font rendering |
| MonoGame.Aseprite | 6.3.1 | Aseprite sprite import |
| Gum.MonoGame | — | UI layout engine |
| BrainAI | — | Pathfinding & AI utilities |
| ImGui.NET | — | Debug UI overlay |
| Coroutine (Ellpeck) | — | Async coroutine support |

## What to Customize

- **GameApp.cs** — Window size, title, vsync, fixed timestep
- **ServiceLocator.cs** — Register your own services (audio, save data, etc.)
- **Scenes/** — Replace placeholders with real scenes
- **Components/** — Add your game-specific components
- **Systems/** — Add your game-specific systems
- **Content/** — Add a `Content/Content.mgcb` via the MGCB Editor

## Architecture Overview

**Scene Manager** drives the game loop. Each `Scene` gets `Initialize → LoadContent → Update/Draw → Unload` lifecycle calls. Scenes own their own ECS worlds via `WorldManager`.

**WorldManager** wraps `Arch.Core.World` and provides a system registration pattern. Systems are plain methods that query the world — no base class needed.

**ServiceLocator** provides static access to shared services (graphics device, content manager, input, etc.) so scenes and systems don't need constructor injection chains.

## Tips

- Components should be **record structs** — small, immutable value types
- Tags are empty structs used for filtering queries (e.g., `PlayerTag`)
- One `WorldManager` per scene keeps ECS worlds isolated
- Use `ServiceLocator` sparingly — it's a convenience, not an architecture
