# G3 — Physics & Collision
> **Category:** Guide · **Related:** [G1 Custom Code Recipes](./G1_custom_code_recipes.md) · [R2 Capability Matrix](../R/R2_capability_matrix.md)

---

## Collision System (Custom)

### Broadphase: SpatialHash

Custom SpatialHash (~80 lines) for fast proximity queries, raycasts, and overlap checks. Implementation in [G1](./G1_custom_code_recipes.md).

Collision shapes are just data, queryable by Arch ECS systems — not locked into a framework's component model.

### Narrowphase: Shape Checks

MonoGame.Extended v5.3.1 includes collision primitives. The maintainer (AristurtleDev) is actively building a new 2D bounding volumes and collision suite based on Ericson's *Real-Time Collision Detection* — being submitted upstream to MonoGame itself for 3.8.6.

Alternatively, they're ~50 lines each to implement directly:
- **AABB vs AABB:** Min/max overlap check
- **Circle vs Circle:** Distance-squared vs sum-of-radii-squared
- **Polygon vs Polygon:** SAT (Separating Axis Theorem)
- **AABB vs Circle:** Clamp circle center to AABB, check distance

Implementation starters in [G1](./G1_custom_code_recipes.md).

---

## Aether.Physics2D (v2.2.0)

Full Box2D port for when you need a real physics simulation. Actively maintained by nkast (MonoGame Foundation member).

**Features:**
- Rigid bodies (dynamic, static, kinematic)
- Joints: revolute, prismatic, distance, weld, rope, gear, pulley, motor
- Raycasting and continuous collision detection (CCD)
- Contact callbacks, sensor bodies
- Stable and well-tested

**Best for:** Physics puzzles, destructible environments, rope/chain mechanics, vehicles, anything needing realistic physics responses.

---

## Custom Verlet Integration (~150-200 lines)

Position-based Verlet with constraints. No library needed.

**Perfect for:** Ropes, cloth, soft bodies, hair, chains, grapple hooks.

**Algorithm:** Store current and previous position. Each frame: compute new position from velocity (current - previous), apply gravity, then iteratively satisfy distance constraints between connected points. Integrate with collision via SpatialHash for ground/wall interaction.

---

## Decision Table: When to Use What

| Need | Solution | Guide |
|------|----------|-------|
| Platformer movement + collision | Custom SpatialHash + AABB checks + custom controller | G1, here |
| Top-down RPG collision | Custom SpatialHash + AABB/circle triggers | G1, here |
| Angry Birds / destructible physics | Aether.Physics2D | here |
| Rope, chain, grapple hook | Custom Verlet integration | here |
| Bullet hell collision | Arch ECS + circle-circle checks (fastest) | here |
| RTS unit collision avoidance | Steering behaviors + SpatialHash | here, [G4](./G4_ai_systems.md) |
| Physics puzzle (Cut the Rope, World of Goo) | Aether.Physics2D (joints, springs, raycasting) | here |
