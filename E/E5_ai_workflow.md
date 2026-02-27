# E5 — AI-Assisted Development Workflow
> **Category:** Explanation · **Related:** [E4 Project Management](./E4_project_management.md) · [E9 Solo Dev Playbook](./E9_solo_dev_playbook.md) · [R3 Project Structure](../R/R3_project_structure.md) · [G11 Programming Principles](../G/G11_programming_principles.md)

---

## Structuring Code for AI

Vertical slice architecture is the most AI-friendly pattern. Organize by feature:

```
Features/
  Combat/
    CombatSystem.cs
    DamageEvent.cs
    ICombatService.cs
    CombatTests.cs
```

This achieves **context isolation** — AI tools can understand a self-contained feature without the entire codebase.

**Rules for AI-friendly code:**
- Keep files under 200-300 lines, one class per file, named identically to the class
- Define interfaces before implementations — AI produces dramatically better code with clear contracts
- Use explicit types over `var` so AI can read type information
- Write XML doc comments on public APIs

---

## CONTEXT.md

Create a CONTEXT.md file in your project root. Feed it to AI with every prompt:

```markdown
# Project: FirePuzzle
## Architecture: MonoGame + Arch ECS + Composed Libraries
## Patterns: Service Locator for ambient services, DI for game logic
## Arch owns: ALL entities (player, NPCs, enemies, particles, simulation)
## Key Libraries: Apos.Input, Gum.MonoGame, FontStashSharp, BrainAI
## Custom Code: Scene manager, render layers, SpatialHash, tweens
## Coding conventions: C# 12, nullable enabled, readonly structs for data
```

---

## What AI Is Good At (Use It For)

- **Boilerplate:** Component classes, interface implementations, data models
- **Test generation:** Unit tests for deterministic systems (damage calc, state machines)
- **Documentation:** XML doc comments, README sections
- **Data file templates:** JSON level definitions, item databases, wave configurations
- **Exploring unfamiliar APIs:** "How do I use Arch command buffers?"
- **Refactoring:** Extracting interfaces, splitting god classes, renaming
- **Pattern implementation:** Give it a pattern description, get a concrete implementation

> **Deep dive:** [E9 Solo Dev Playbook](./E9_solo_dev_playbook.md) — realistic productivity data (10–20% gains), ECS-specific AI synergies, cognitive atrophy risk, brainstorming as top non-code use

---

## What AI Is Bad At (Write It Yourself)

- Core game loop and fixed timestep integration → [G15](../G/G15_game_loop.md)
- Physics and collision resolution edge cases → [G3](../G/G3_physics_and_collision.md)
- State machine transitions with subtle timing requirements
- Performance-critical inner loops (measure, don't trust AI's optimization instincts)
- Anything involving your game's unique "feel" — jump arcs, attack timing, camera behavior

> **Deep dive:** [E9 Solo Dev Playbook](./E9_solo_dev_playbook.md) — AI art pipeline (70/30 rule, img2img workflow, LoRA training, ComfyUI), "AI slop" reputational risk

---

## AI Code Review Checklist

AI-generated code has systematic failure patterns. Check every piece for:

1. **Hallucinated APIs** — methods/classes that don't exist in your libraries
2. **Performance anti-patterns** — O(n²) where O(n) exists, LINQ in hot paths, string concatenation in loops → [G13](../G/G13_csharp_performance.md)
3. **Missing edge cases** — null inputs, empty collections, boundary values, integer overflow
4. **Incorrect error handling** — swallowing exceptions, catching too broadly
5. **Stale patterns** — using obsolete APIs from older .NET versions
6. **Memory leaks** — event handlers not unsubscribed, async tasks not cancelled → [G13](../G/G13_csharp_performance.md)
7. **Thread safety assumptions** — AI often ignores concurrency concerns

---

## Workflow

1. Write the interface/contract yourself
2. Ask AI to implement it, providing CONTEXT.md and relevant files
3. Review output against the checklist above
4. Run it — verify behavior matches intent
5. Commit immediately after each successful chunk
6. Schedule regular refactoring sprints — AI generates "good enough" not optimal

**Spend 5 minutes reviewing for every 1 minute of generation.** Technical debt accumulates faster with AI assistance because AI code is locally correct but globally incoherent — it doesn't know your architectural vision.
