# Universal 2D Engine Toolkit

**Build games, not frameworks.** A comprehensive knowledge base for building 2D games with MonoGame + Arch ECS + composed libraries in C#.

## What Is This?

92 documents covering every aspect of 2D game development — from architecture decisions to shipping on Steam. Instead of one monolithic engine, this toolkit teaches you to compose focused libraries into exactly the engine your game needs.

### The Stack

```
MonoGame.Framework.DesktopGL     — Base framework (rendering, audio, input, content)
Arch ECS (v2.1.0)               — Entity Component System for all game objects
MonoGame.Extended (v5.3.1)       — Camera, Tiled maps, collision shapes, math
Gum.MonoGame                     — UI framework
Apos.Input (v2.5.0)             — Input handling
FontStashSharp.MonoGame (v1.3.7) — Runtime font rendering
MonoGame.Aseprite (v6.3.1)      — Sprite animation from .aseprite files
Aether.Physics2D (v2.2.0)       — Box2D-style physics
BrainAI                          — FSM, Behavior Trees, GOAP, pathfinding
ImGui.NET                        — Debug UI and tooling
```

## Documentation Structure

Organized using the [Diátaxis framework](https://diataxis.fr/):

| Category | Count | Purpose |
|----------|-------|---------|
| **Reference** (R1–R4) | 4 | Library specs, capability matrix, project structure |
| **Explanation** (E1–E9) | 8 | Architecture decisions, workflow philosophy |
| **Guides** (G1–G63) | 63 | Step-by-step implementation for every system |
| **Catalog** (C1–C2) | 2 | Genre mapping, game feel reference |
| **Playbook** (P0–P15) | 16 | Production checklists, templates, launch pipeline |

→ **[Browse the full index](INDEX.md)**

## Guides Cover

**Core Engine:** Game loop · Scene management · Resource loading · Content pipeline · Display/resolution · Hot reload

**Rendering:** SpriteBatch pipeline · Shaders & VFX · Particles · 2D lighting & shadows · Parallax · Screen transitions · Trails & lines · Water simulation

**Gameplay:** Character controller · Physics & collision · Tilemap systems · Pathfinding (A*, flow fields) · AI (FSM, BT, GOAP) · Tweening · Animation state machines · Skeletal animation (Spine)

**Game Systems:** Inventory · Dialogue · Save/load · Crafting · Quests · Achievements · Procedural generation · Fog of war · Weather · Cutscenes · Narrative/branching story · Tutorial/onboarding

**Perspectives:** Side-scrolling · 3/4 top-down · Isometric

**UI & Input:** Gum UI framework · Input handling · Settings menu · Minimap · Safe areas

**Quality:** C# performance · Design patterns · Debugging · Testing · Profiling

**Shipping:** Deployment · Publishing (Steam/itch/iOS) · Localization · Accessibility · Crash reporting · Version control · Modding support · Online services

## Playbook

The Playbook is your production companion — 16 documents covering the entire journey from idea to launch:

- **Master Playbook** — 7-phase journey from ideation to post-launch
- **Pre-Production** — Design doc, scope worksheet, art style decisions
- **Milestones** — Vertical slice → Alpha → Beta → RC → Gold checklists
- **Art & Audio Pipelines** — Asset creation workflows
- **GDD Template** — Fillable game design document
- **Integration Map** — System dependency graph and build order
- **Polish Checklist** — Comprehensive juice & game feel checklist
- **Launch Checklist** — Store pages, trailers, press kit, launch day
- **Marketing Timeline** — 12-month marketing plan

## Code Examples

The `examples/` directory contains standalone C# files extracted from the guides:

- **Core** — Scene management, services
- **Character** — Platformer character controller, collision resolution
- **Tilemap** — Rendering, collision, autotiling
- **Lighting** — Lightmap system, shaders
- **Pathfinding** — A*, flow fields
- **Procgen** — BSP dungeons, cellular automata caves
- **FogOfWar** — Recursive shadowcasting
- **Effects** — Trail rendering, rope physics
- **Tween** — Easing functions, tween engine

All code uses `record struct` ECS components with Arch v2.1.0.

## Quick Start

1. **Read** [E1 Architecture Overview](E/E1_architecture_overview.md) to understand the philosophy
2. **Install** packages from [R1 Library Stack](R/R1_library_stack.md)
3. **Set up** your project with [R3 Project Structure](R/R3_project_structure.md)
4. **Pick your genre** from [C1 Genre Reference](C/C1_genre_reference.md)
5. **Check the build order** in [P10 Integration Map](Playbook/10_integration_map.md)
6. **Build systems** using the relevant G-docs
7. **Ship it** with [P0 Master Playbook](Playbook/00_master_playbook.md)

## Who Is This For?

Solo developers and small teams building 2D games with C# who want:
- Full control over their engine (no black boxes)
- Modern ECS architecture (Arch is fast and ergonomic)
- Practical, code-heavy documentation (not theory)
- A clear path from prototype to shipped game

## License

This documentation is provided as-is for personal and educational use.
