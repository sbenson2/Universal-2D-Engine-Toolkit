# G9 — Networking
> **Category:** Guide · **Related:** [R1 Library Stack](../R/R1_library_stack.md) · [C1 Genre Reference](../C/C1_genre_reference.md) · [G15 Game Loop](./G15_game_loop.md)

---

## LiteNetLib (Primary)

**Install:** `dotnet add package LiteNetLib`

### Features
- Reliable and unreliable UDP
- Connection management
- NAT traversal
- Serialization helpers

**Best for:** Co-op, competitive multiplayer, lobbies.

---

## Rollback Netcode (Custom — Fighting Games)

Required for fighting games and fast-paced competitive games. No off-the-shelf C# library — must implement using GGPO concepts.

### How It Works
- Store game state snapshots in a ring buffer → [G14](./G14_data_structures.md)
- On desync: rewind to last confirmed state, replay inputs forward
- Demands deterministic simulation (fixed timestep, no floating point variance)

### Requirements
- Fixed timestep game loop → [G15](./G15_game_loop.md)
- Deterministic physics and game logic
- State snapshot/restore capability
- Input prediction and rollback

---

## Client-Server Authority

For non-fighting multiplayer (RTS, co-op, MMO-style):

- Server validates all actions, clients predict locally
- **Delta compression:** Send only changes from last acknowledged state (50-80% bandwidth reduction)
- **Interpolation buffer:** Render 2-3 snapshots behind server time for smooth visuals
- **Interest management:** Only send nearby entity data (critical for large worlds)

---

## Bandwidth Reduction

**Delta compression:** Send only changes from last acknowledged state. 50-80% reduction in typical games.

**Quantization:** Positions in 24 bits instead of 32-bit floats. Quaternion rotations using "smallest three" encoding at 29 bits. Choose precision based on gameplay needs — pixel-art games can often quantize to integer positions.

**Interest management:** Only send data for entities near the player. Combine with spatial partitioning → [G1](./G1_custom_code_recipes.md).

---

## Client-Side Prediction

Apply inputs locally immediately while awaiting server confirmation. Store input history in a ring buffer → [G14](./G14_data_structures.md). When server response arrives, if position differs beyond threshold, snap to server position and replay all inputs since the confirmed tick.

This hides latency for the local player while keeping the server authoritative.

---

## Interpolation

Buffer 2-3 server snapshots and interpolate between them, running ~100ms behind authoritative time to hide network jitter. This means remote entities appear smooth even with packet loss or irregular delivery.

---

## Fixed-Point Math (Deterministic Networking)

Fixed-point stores numbers as integers with a fixed number of fractional bits (typically Q16.16 = 65,536 subdivisions per unit). Guarantees identical results across all CPUs, compilers, and platforms.

**Required for:** Deterministic lockstep networking in RTS and fighting games where only inputs are sent over the network.

**Cost:** 4-10x slower than floating-point (no hardware trig, limited SIMD), limited range (~±32K with Q16.16), incompatible with engine built-ins.

**Library:** FixedMath.Net (Fix64, Q31.32 format with lookup tables for trig).

**The core problem:** IEEE 754 guarantees identical results for basic operations (+, -, *, /) on the same platform, but transcendental functions (sin, cos, sqrt) are implementation-defined. Even basic operations can differ across platforms due to compiler FMA (fused multiply-add) optimizations.

**Verdict:** If you need cross-platform deterministic networking, fixed-point is necessary. For single-player or server-authoritative multiplayer, floating-point with server reconciliation is simpler and faster.

---

## Version Control for Game Assets

Git LFS tracks binary assets — configure .gitattributes for .psd, .png, .wav, .ogg, .fbx, and other large formats before first commit. File locking prevents concurrent binary edits. Short-lived feature branches — long-lived branches with binary assets create merge nightmares.

---

## Genre Networking Needs

| Genre | Model | Notes |
|-------|-------|-------|
| Fighting game | Rollback | Custom implementation, LiteNetLib transport |
| Co-op action | Client-server | Server authority, client prediction |
| RTS | Lockstep | Deterministic simulation, send only inputs, fixed-point math |
| Turn-based | Request/response | Simplest model, can use TCP |
| MMO-style | Client-server | Interest management, delta compression |
