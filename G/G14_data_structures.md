# G14 — Data Structures
> **Category:** Guide · **Related:** [G1 Custom Code Recipes](./G1_custom_code_recipes.md) · [G13 C# Performance](./G13_csharp_performance.md) · [G3 Physics & Collision](./G3_physics_and_collision.md)

---

Game-specific data structures beyond standard collections.

---

## Spatial Data Structures — Choosing the Right One

For most 2D games, a uniform grid or spatial hash outperforms quadtrees. Grids provide O(1) insertion and lookup via direct indexing, while quadtrees require pointer-chasing through tree nodes (cache-unfriendly).

**By entity count:**
- **~100 entities:** Brute-force O(n²) is only 10,000 checks — likely faster than maintaining any structure
- **~1,000 entities:** Spatial hashing wins handily
- **~10,000+ entities:** Spatial hashing dominates for uniform-size objects; loose quadtrees excel for varied-size entities

**By game type:**
- **Platformer:** Brute force or simple grid (few collision checks)
- **Bullet hell:** Spatial hashing (thousands of same-size projectiles)
- **RTS:** Loose quadtree (varied unit sizes, range queries)
- **Cellular automata:** Direct grid indexing (fixed positions)

Full SpatialHash implementation: → [G1 Custom Code Recipes](./G1_custom_code_recipes.md)

---

## Ring Buffers

Fixed-size, zero allocation after initialization, O(1) push/read. Essential for input history, replay systems, frame state buffering, and structured logging.

```csharp
public class RingBuffer<T>
{
    private readonly T[] _buffer;
    private int _head;
    private int _count;

    public RingBuffer(int capacity) => _buffer = new T[capacity];

    public int Count => _count;
    public int Capacity => _buffer.Length;

    public void Push(T item)
    {
        _buffer[_head] = item;
        _head = (_head + 1) % _buffer.Length;
        if (_count < _buffer.Length) _count++;
    }

    // Index 0 = most recent, 1 = previous, etc.
    public T Get(int ageIndex)
    {
        if (ageIndex >= _count) throw new IndexOutOfRangeException();
        int index = (_head - 1 - ageIndex + _buffer.Length) % _buffer.Length;
        return _buffer[index];
    }
}

// Usage: input history for combo detection
var inputHistory = new RingBuffer<InputFrame>(60); // 1 second at 60fps
```

**For replay/rewind state buffers:** Reuse game state objects instead of cloning them. The simulation should take two GameState objects (previous and write-here) to avoid GC pressure.

---

## Priority Queue (.NET 6+)

Min-heap with O(log n) enqueue/dequeue, O(1) peek. For A* pathfinding, insert duplicates and skip stale entries when dequeuing instead of implementing decrease-key:

```csharp
var frontier = new PriorityQueue<Vector2I, float>();
frontier.Enqueue(start, 0f);

while (frontier.Count > 0)
{
    var current = frontier.Dequeue();
    if (current == goal) break;
    if (visited.Contains(current)) continue; // Skip stale entries
    visited.Add(current);

    foreach (var next in GetNeighbors(current))
    {
        float newCost = costSoFar[current] + GetMoveCost(current, next);
        if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
        {
            costSoFar[next] = newCost;
            float priority = newCost + Heuristic(next, goal);
            frontier.Enqueue(next, priority);
        }
    }
}
```

Also useful for event scheduling: key on scheduledTime, dequeue all events where priority ≤ currentTime.

---

## Bit Manipulation and Collision Flags

Efficient collision layer filtering — a single bitwise AND determines if two objects should interact in O(1):

```csharp
[Flags]
public enum CollisionLayer : uint
{
    None         = 0,
    Player       = 1 << 0,   // 1
    Enemy        = 1 << 1,   // 2
    PlayerBullet = 1 << 2,   // 4
    EnemyBullet  = 1 << 3,   // 8
    Terrain      = 1 << 4,   // 16
    Pickup       = 1 << 5,   // 32
}

// Set what each layer collides WITH
var playerMask = CollisionLayer.Enemy | CollisionLayer.EnemyBullet
               | CollisionLayer.Terrain | CollisionLayer.Pickup;
var enemyMask  = CollisionLayer.Player | CollisionLayer.PlayerBullet
               | CollisionLayer.Terrain;

// Check collision: O(1)
bool shouldCollide = (entityA.Layer & entityB.Mask) != 0
                  && (entityB.Layer & entityA.Mask) != 0;
```

For ECS component masks:
```csharp
// Match entities to systems in a single instruction
bool matches = (entity.ComponentMask & system.RequiredMask) == system.RequiredMask;
```

`HasFlag()` in .NET Core+ is JIT-optimized to a bitwise AND (no longer boxes like pre-.NET Core).
