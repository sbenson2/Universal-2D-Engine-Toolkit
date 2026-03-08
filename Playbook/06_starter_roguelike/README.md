# 06 — Roguelike Starter Kit

A complete, copy-paste-ready roguelike template built on top of the [Project Template](../01_project_template/). Energy-based turns, grid movement, procedural dungeons, shadowcast FOV, bump combat, and a scrolling message log — everything you need for a traditional roguelike.

## Prerequisites

- **01_project_template** — provides `GameApp`, `SceneManager`, `Scene`, `WorldManager`, `ServiceLocator`
- **MonoGame.Framework.DesktopGL**
- **Arch** ECS v2.1.0 (`dotnet add package Arch`)
- **Apos.Input** (`dotnet add package Apos.Input`)
- A `SpriteFont` named `"DefaultFont"` in your Content pipeline (optional — game works without it, but HUD text won't render)

## Quick Start

1. Start with the project template from `01_project_template/`
2. Copy this entire folder's `.cs` files into your `src/` directory
3. In `GameApp.LoadContent()`, change the initial scene:

```csharp
SceneManager.Push(new MyGame.Roguelike.Scenes.DungeonScene());
```

4. Build and run — you'll spawn in a procedurally generated dungeon

## Controls

| Key | Action |
|---|---|
| **WASD** / **Arrows** | Move (grid-based) |
| **Period** / **Numpad 5** | Wait one turn |
| **E** | Interact (pick up items, descend stairs) |

## File Overview

### Components (ECS data)
| File | Purpose |
|---|---|
| `Components/TurnActor.cs` | Energy-based turn tracking (speed, energy, energy-per-turn) |
| `Components/GridPosition.cs` | Tile-based (x, y) position |
| `Components/Stats.cs` | HP, attack, defense, level, EXP |
| `Components/FieldOfView.cs` | Vision radius + currently visible tile set |
| `Components/AiIntent.cs` | AI behavior mode (Wander / Chase / Flee) |
| `Components/Inventory.cs` | Item storage with max capacity |

### Tags (zero-size markers)
| File | Purpose |
|---|---|
| `Tags/PlayerTag.cs` | Player-controlled entity |
| `Tags/EnemyTag.cs` | Hostile entity |
| `Tags/ItemTag.cs` | Pickup-able item |
| `Tags/BlocksMovementTag.cs` | Entity blocks tile movement |
| `Tags/BlocksSightTag.cs` | Entity blocks line of sight |

### Map
| File | Purpose |
|---|---|
| `Map/TileType.cs` | Enum: Floor, Wall, StairsDown, Door |
| `Map/GameMap.cs` | 2D tile array with walkability, opacity, fog state |
| `Map/DungeonGenerator.cs` | Room-and-corridor generation with seeded RNG |
| `Map/ShadowcastFOV.cs` | 8-octant recursive shadowcasting (from G54) |

### Systems
| File | Purpose |
|---|---|
| `Systems/TurnSystem.cs` | Energy accumulation, turn ordering, player-wait |
| `Systems/PlayerInputSystem.cs` | WASD/arrow movement via Apos.Input |
| `Systems/AiSystem.cs` | Wander/chase/flee behavior selection |
| `Systems/MovementSystem.cs` | Grid movement + bump-attack detection |
| `Systems/CombatSystem.cs` | Damage calc, death, EXP/level-up |
| `Systems/FovSystem.cs` | Runs shadowcast, updates map visibility |
| `Systems/RenderSystem.cs` | Tile + entity rendering with fog-of-war |
| `Systems/HudSystem.cs` | HP bar, level, depth, message log overlay |

### Scene & Config
| File | Purpose |
|---|---|
| `Scenes/DungeonScene.cs` | Wires everything: dungeon gen, spawning, turn loop |
| `RoguelikeConfig.cs` | All tuning constants in one place |
| `MessageLog.cs` | Scrolling combat/event message log |

## Architecture

```
DungeonScene (orchestrator)
 ├─ WorldManager (Arch ECS world)
 ├─ GameMap (tile data + fog state)
 ├─ DungeonGenerator → produces GameMap + spawn points
 ├─ TurnSystem (energy loop)
 │   ├─ Player turn → PlayerInputSystem → MovementSystem / CombatSystem
 │   └─ Enemy turn → AiSystem → MovementSystem / CombatSystem
 ├─ FovSystem → ShadowcastFOV → updates GameMap visibility
 ├─ RenderSystem (map + entities)
 ├─ HudSystem (stats + log)
 └─ MessageLog (event text)
```

### Turn Flow

1. `TurnSystem` grants energy to all actors each tick
2. When an actor reaches 100 energy, it's their turn
3. **Player turn**: system pauses, waits for input via `PlayerInputSystem`
4. **Enemy turn**: `AiSystem` picks a behavior, `MovementSystem` executes it
5. After acting, 100 energy is consumed and the loop continues
6. `FovSystem` recalculates visibility after the player moves

### Combat

Moving into a tile occupied by an enemy triggers a **bump attack**:
- Damage = `max(1, attacker.Attack - defender.Defense)`
- On kill: entity is destroyed, EXP awarded, level-up checked
- Level up: +5 MaxHP (full heal), +1 Attack, +1 Defense

## Customization Guide

### Add a new enemy type
1. Add stats to `RoguelikeConfig.cs`
2. In `DungeonScene.GenerateFloor()`, add spawn logic
3. Optionally extend `AiSystem` with new behaviors

### Add items with effects
1. Create a new component (e.g. `HealingItem`, `WeaponItem`)
2. Attach it when spawning items in `DungeonScene`
3. Handle usage in `DungeonScene.TryInteract()` or a new `ItemSystem`

### Customize dungeon generation
- Tweak `RoguelikeConfig` values (room count, size, map dimensions)
- Swap `DungeonGenerator` for BSP, cellular automata, or WFC (see G53)
- Add room templates for handcrafted content mixed with procedural layout

### Add more FOV features
- Give enemies their own `FieldOfView` component for stealth mechanics
- Use `BlocksSightTag` on entities (closed doors, large creatures)
- See G54_fog_of_war.md for shader-based smooth fog rendering

### Change rendering
- Replace colored rectangles with sprite textures
- Add tile atlases with `SpriteBatch.Draw(texture, sourceRect, destRect)`
- Layer floor decorations, wall variations, entity sprites

## Related Guides

- **G53_procedural_generation.md** — BSP, cellular automata, WFC, seeded random
- **G54_fog_of_war.md** — shadowcasting algorithm, fog rendering, visibility states
- **01_project_template/** — base project structure this kit extends
