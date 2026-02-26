# G10 — Custom Game Systems
> **Category:** Guide · **Related:** [C1 Genre Reference](../C/C1_genre_reference.md) · [R3 Project Structure](../R/R3_project_structure.md)

---

These are systems no library provides well enough — you'll write them as reusable modules in your Core project. Each is genre-agnostic and composable. Store them in `src/Systems/` per the [project structure](../R/R3_project_structure.md).

---

## 1. Inventory System (~500-800 lines)

- Slot-based grid (Terraria) or list-based (RPG)
- Item stacking with max stack sizes
- Item categories, rarity, equip slots
- Drag-and-drop via Gum UI → [G5](./G5_ui_framework.md) or custom input handling
- Serializable for save/load

**No NuGet library is production-ready for game inventories.** Roll your own.

---

## 2. Dialogue System (~400-600 lines)

- Node-based dialogue tree (speaker, text, choices, conditions, effects)
- Variable/flag tracking for conditional branches
- Typewriter text rendering (FontStashSharp for display)
- Portrait/expression switching
- Data format: JSON or custom markup

**Consider:** Ink runtime (C# library) for complex branching narratives. Otherwise, a custom JSON-based node graph covers most needs.

---

## 3. Save/Load System (~300-500 lines)

- Collect `ISaveable` data from all registered objects
- Serialize to JSON via System.Text.Json (with source generators for AOT)
- Version migration for save compatibility between builds
- Slot-based save files
- Arch.Persistence for ECS world snapshots (install from [R1](../R/R1_library_stack.md))

---

## 4. Procedural Generation Suite (~200-400 lines each)

All pure C# algorithms, no library needed:

| Algorithm | Use Case | ~Lines |
|-----------|----------|--------|
| **BSP Dungeon** | Room-and-corridor dungeons (roguelikes) | 200 |
| **Cellular Automata** | Cave generation, organic terrain | 150 |
| **Drunk Walk / Random Walk** | Simple connected cave passages | 100 |
| **Wave Function Collapse** | Tile-based constraint propagation for complex maps | 400 |
| **Poisson Disk Sampling** | Even distribution of objects (trees, rocks, enemies) | 150 |
| **Perlin/Simplex Noise** | Terrain height, biome distribution | 200 |

Use seed-based RNG (`System.Random` with explicit seed) for reproducible generation.

---

## 5. Crafting System (~300 lines)

- Recipe database: `inputs[]` → `output`
- Crafting station types (forge, alchemy table, workbench)
- Recipe discovery/unlock progression
- UI: ingredient slots + output preview (Gum → [G5](./G5_ui_framework.md))

---

## 6. Quest / Objective System (~400-600 lines)

- Quest states: unavailable → available → active → complete → turned in
- Objective types: kill X, collect Y, reach location, talk to NPC
- Reward distribution on completion
- Quest log UI

---

## 7. Status Effect / Buff System (~300-500 lines)

- Timed effects: poison (DOT), shield (absorb), speed boost, stun
- Stackable vs refreshable vs unique
- Visual indicators (icons, tints, particles)
- Modifies stats through a modifier stack: `final = (base + flat bonuses) * multiplier`

**Critical for:** RPG, roguelike, card games.

---

## 8. Undo/Redo — Command Pattern (~100-200 lines)

```csharp
public interface ICommand
{
    void Execute();
    void Undo();
}
```

- Command history stack with redo branch pruning
- **Essential for:** Puzzle games, strategy games, level editors

---

## 9. Wave / Spawn System (~200-300 lines)

- Wave definitions: enemy types, counts, spawn intervals, spawn points
- Inter-wave timers
- Difficulty scaling (multipliers per wave)
- Data-driven via JSON in `Resources/waves/`

**Used by:** Tower defense, survival, arena games.

---

## 10. Day/Night and Weather (~200-400 lines)

- Time-of-day cycle (0-24h mapped to configurable real-time duration)
- Ambient light color/intensity curves (drive via post-processor or tint)
- Weather states: clear, rain, snow, fog (particle systems + shader tints)
- Gameplay effects: crop growth, enemy spawns, NPC schedules
