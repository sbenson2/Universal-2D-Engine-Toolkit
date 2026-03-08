# 05 — Top-Down RPG Starter Kit

A **copy-paste-ready** top-down RPG foundation built on the project template from `01_project_template/`. Provides 8-directional movement, AABB slide collision, NPC interaction, a typewriter dialogue system, item inventory, and a smooth-follow camera — everything you need to start building a Zelda/Stardew-style overworld.

> **Stack:** MonoGame.Framework.DesktopGL · Arch ECS v2.1.0 · Apos.Input · C# 12+

---

## How to Use

### 1. Start from the Project Template

Copy the `01_project_template/` folder as your project base. It provides:

- `GameApp` — main game loop, graphics setup, service registration
- `SceneManager` — push/pop scene stack
- `Scene` — abstract base with Initialize → LoadContent → Update/Draw → Unload
- `WorldManager` — Arch ECS world + system registration
- `ServiceLocator` — lightweight service container
- Core components: `Position`, `Velocity`, `Sprite`

### 2. Layer This Kit On Top

Copy the folders from this starter kit into your project's `src/` directory:

```
src/
├── TopDown/
│   ├── Components/     ← CharacterBody, CharacterMotion, FacingDirection, etc.
│   ├── Tags/           ← PlayerTag, NpcTag, SolidTag, InteractableTag
│   ├── Systems/        ← InputSystem, MovementSystem, CollisionSystem, etc.
│   ├── Dialogue/       ← DialogueBox, DialogueData
│   ├── Inventory/      ← InventoryManager, ItemDatabase
│   ├── Scenes/         ← OverworldScene (starter scene)
│   └── TopDownConfig.cs
```

### 3. Add NuGet Dependencies

Ensure your `.csproj` includes:

```xml
<PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.*" />
<PackageReference Include="Arch" Version="2.1.0" />
<PackageReference Include="Apos.Input" Version="*" />
```

### 4. Wire Up Apos.Input

In your `GameApp.Initialize()`:

```csharp
Apos.Input.InputHelper.Setup(this);
```

In your `GameApp.Update()`, wrap the scene update:

```csharp
Apos.Input.InputHelper.UpdateSetup();
SceneManager.Update(gameTime);
Apos.Input.InputHelper.UpdateCleanup();
```

### 5. Switch to the Overworld Scene

In `GameApp.LoadContent()`, push the overworld instead of the default scene:

```csharp
SceneManager.Push(new MyGame.TopDown.Scenes.OverworldScene());
```

---

## File Overview

### Components (record structs — data only, no logic)

| File | Purpose |
|------|---------|
| `CharacterBody.cs` | AABB collision footprint (width × height at feet) |
| `CharacterMotion.cs` | Move speed, acceleration, friction tuning |
| `FacingDirection.cs` | Current facing (X, Y) for 4/8-directional |
| `AnimationState.cs` | Current animation key + horizontal flip |
| `Stats.cs` | HP, Attack, Defense, Speed, Level, Exp |
| `Inventory.cs` | List of item IDs held by an entity |
| `DialogueSpeaker.cs` | NPC name, portrait key, dialogue data ID |
| `Interactable.cs` | Interaction radius + action type |

### Tags (empty record structs for query filtering)

| File | Purpose |
|------|---------|
| `PlayerTag.cs` | Identifies the player entity |
| `NpcTag.cs` | Identifies NPC entities |
| `SolidTag.cs` | Marks entities as collision obstacles |
| `InteractableTag.cs` | Marks entities the player can interact with |

### Systems (static classes, `Update(World, GameTime)` signature)

| File | Purpose |
|------|---------|
| `InputSystem.cs` | WASD/Arrow input → velocity + facing direction |
| `TopDownMovementSystem.cs` | Applies velocity × dt to position |
| `CollisionSystem.cs` | AABB slide collision (try X, then Y separately) |
| `InteractionSystem.cs` | Proximity check + interact button → triggers dialogue/actions |
| `AnimationStateSystem.cs` | Derives walk_up/down/side and idle from velocity + facing |
| `CameraFollowSystem.cs` | Smooth lerp follow with bounds clamping + pixel snap |

### Dialogue

| File | Purpose |
|------|---------|
| `DialogueData.cs` | Data structures: `DialogueData`, `DialogueLine`, `DialogueChoice` |
| `DialogueBox.cs` | UI class: typewriter text reveal, advance on button, speaker name |

### Inventory

| File | Purpose |
|------|---------|
| `ItemDatabase.cs` | Static item definitions (`ItemDefinition` record) + registry |
| `InventoryManager.cs` | Add/remove/has/count items — service-level class |

### Scene

| File | Purpose |
|------|---------|
| `OverworldScene.cs` | Wires all systems, spawns player + NPCs, creates walls |

### Config

| File | Purpose |
|------|---------|
| `TopDownConfig.cs` | All tuning constants in one place |

---

## System Execution Order

The order systems are registered in `OverworldScene.LoadContent()` matters:

```
1. InputSystem          — reads keyboard, sets velocity + facing
2. TopDownMovementSystem — integrates velocity into position
3. CollisionSystem       — resolves AABB overlaps with solids
4. InteractionSystem     — checks interact button + proximity
5. AnimationStateSystem  — derives animation from velocity + facing
6. CameraFollowSystem    — smoothly follows player position
```

---

## Key Design Decisions

### Diagonal Normalization
Input direction vector is normalized before applying speed, so diagonal movement isn't ~41% faster than cardinal movement. This is handled in `InputSystem`.

### Slide Collision
`CollisionSystem` resolves X and Y axes separately. If you walk into a wall at an angle, you slide along it instead of stopping dead. The system collects all `SolidTag` entities into a list and checks each mover against them.

### Collision = Ground Footprint
Per G28 (top-down perspective guide): collision boxes represent the **ground-plane footprint**, not the visual sprite. A 16×32 character sprite gets ~10×6 pixel collision at the feet. This is why `CharacterBody` defaults are small.

### Animation Convention
Art provides 3 directional sets: **down**, **up**, **side** (facing right). Left-facing reuses the side art with `FlipX = true`. Animation keys follow the pattern: `{action}_{direction}` — e.g., `idle_down`, `walk_side`, `walk_up`.

### Camera Pixel Snap
`CameraFollowSystem` rounds position to whole pixels after smoothing to prevent sub-pixel jitter in pixel art. Bounds clamping happens after smoothing (per G28) to prevent edge jitter.

---

## Customization Guide

### Changing Movement Speed
Edit `TopDownConfig.DefaultMoveSpeed` (pixels/second). The default 80 px/s feels like a classic RPG walk speed.

### Adding New NPCs
In `OverworldScene.LoadContent()`, create a new entity with the NPC archetype:

```csharp
world.Create(
    new Position(x, y),
    new Velocity(0f, 0f),
    new CharacterBody(10f, 6f),
    new FacingDirection(0, 1),
    new AnimationState("idle_down", false),
    new DialogueSpeaker("Name", "portrait_key", "dialogue_id"),
    new Interactable(TopDownConfig.DefaultInteractionRadius, "dialogue"),
    new NpcTag(),
    new InteractableTag(),
    new SolidTag()
);
```

### Adding New Items
Register items in `ItemDatabase`:

```csharp
ItemDatabase.Register(new ItemDefinition("magic_ring", "Magic Ring", "Boosts defense by 2.", ItemType.Armor, 2));
```

### Adding Dialogue
Add entries to the dialogue database dictionary in the scene:

```csharp
dialogueDb["new_dialogue_id"] = new DialogueData
{
    Id = "new_dialogue_id",
    Lines = new List<DialogueLine>
    {
        new() { Text = "First line of dialogue." },
        new() { Text = "Second line." }
    }
};
```

### Branching Dialogue
Use `DialogueChoice` on a line to offer player choices:

```csharp
new DialogueLine
{
    Text = "Will you help me?",
    Choices = new List<DialogueChoice>
    {
        new() { Label = "Yes", JumpToDialogueId = "quest_accept" },
        new() { Label = "No", JumpToDialogueId = "quest_decline" }
    }
}
```

> **Note:** Choice selection UI is not included in the starter kit — extend `DialogueBox` to render and navigate choices.

### Extending to a Full RPG
This kit is the foundation. Next steps to build on it:

- **Tile map rendering** — load TMX maps via DotTiled, draw ground/overlay layers
- **Sprite rendering** — load Aseprite files via MonoGame.Aseprite, draw entities Y-sorted by foot position
- **Combat system** — add attack states, hitbox entities, damage calculation using Stats
- **Scene transitions** — fade to black, load new maps, place player at spawn points
- **Save/load** — serialize player position, stats, inventory, quest flags
- **Quest system** — flag-based quest tracking with NPC dialogue branching on state

---

## References

- **G28 — Top-Down Perspective** (`G/G28_top_down_perspective.md`) — deep dive on 3/4 view rendering, Y-sorting, collision shapes, camera systems
- **01 — Project Template** (`Playbook/01_project_template/`) — base architecture this kit builds on
