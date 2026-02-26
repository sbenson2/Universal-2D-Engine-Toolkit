# G4 — AI Systems
> **Category:** Guide · **Related:** [R2 Capability Matrix](../R/R2_capability_matrix.md) · [C1 Genre Reference](../C/C1_genre_reference.md)

---

## BrainAI (Primary)

Standalone library extracted from Nez's AI code by the same community. Pure C#, no framework dependency. Provides everything Nez offered:

- **Finite State Machines (FSM):** Simple state + transition model
- **Behavior Trees:** Selector, sequence, decorator, leaf nodes
- **GOAP (Goal-Oriented Action Planning):** Actions with preconditions/effects, planner finds optimal action sequence
- **Utility AI:** Score-based action selection (hunger, fear, aggression curves)
- **Pathfinding:** A*, Dijkstra, BFS on grids or custom graphs
- **Influence Maps:** Spatial scoring for strategic decisions

**Install:** Clone from GitHub (no NuGet package — add as source reference or local package).

---

## Roy-T.AStar (Pathfinding Alternative)

NuGet available, .NET Standard, no dependencies. Use when you only need pathfinding without the full AI suite.

**Install:** `dotnet add package RoyT.AStar`

---

## Genre-Specific AI Patterns

| Genre | AI Approach | Notes |
|-------|-----------|-------|
| Platformer enemies | FSM (patrol → chase → attack → flee) | Simple, predictable |
| RPG town NPCs | Scheduled behavior + dialogue triggers | Time-of-day routines |
| RTS units | Behavior trees + influence maps + flow fields | Complex, layered |
| Stealth game guards | FSM with perception system (vision cones, sound propagation) | Sensory input |
| Card game opponent | Minimax / Monte Carlo Tree Search | Custom implementation |
| Tower defense creeps | Follow pre-computed path, no decision-making | Trivial AI |
| Boss patterns | Hierarchical state machine with phase transitions | State nesting |

---

## Integration with Arch ECS

AI decisions feed into Arch components. A typical pattern:

1. **AI System** queries entities with `AIComponent + Position + Velocity`
2. Runs BrainAI FSM/BT/GOAP to produce a decision
3. Writes the decision result to the entity's components (e.g., sets `Velocity`, changes `AIState`)
4. **Movement System** reads `Velocity` and updates `Position`

This keeps AI logic separate from movement/rendering, testable in isolation.
