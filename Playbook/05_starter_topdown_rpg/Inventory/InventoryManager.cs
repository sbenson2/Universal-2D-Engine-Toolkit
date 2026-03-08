namespace MyGame.TopDown.Inventory;

/// <summary>
/// Service for managing a player's inventory. Wraps add/remove/query operations
/// on a flat list of item IDs. For ECS integration, update the entity's
/// <see cref="Components.InventoryComponent"/> after changes.
/// </summary>
/// <remarks>
/// This is a service-level class (register via ServiceLocator) rather than an ECS system,
/// because inventory mutations are event-driven (not per-frame).
/// For stackable items, upgrade the internal storage to a Dictionary&lt;string, int&gt;.
/// </remarks>
public class InventoryManager
{
    private readonly List<string> _items = new();

    /// <summary>Number of items currently held.</summary>
    public int Count => _items.Count;

    /// <summary>Read-only view of all item IDs.</summary>
    public IReadOnlyList<string> Items => _items;

    /// <summary>
    /// Add an item by ID.
    /// </summary>
    /// <returns>True if added. Override for capacity limits.</returns>
    public bool AddItem(string itemId)
    {
        _items.Add(itemId);
        return true;
    }

    /// <summary>
    /// Remove the first occurrence of an item by ID.
    /// </summary>
    /// <returns>True if the item was found and removed.</returns>
    public bool RemoveItem(string itemId)
    {
        return _items.Remove(itemId);
    }

    /// <summary>
    /// Check if the inventory contains at least one of the given item.
    /// </summary>
    public bool HasItem(string itemId)
    {
        return _items.Contains(itemId);
    }

    /// <summary>
    /// Count how many of a specific item are held.
    /// </summary>
    public int CountItem(string itemId)
    {
        int count = 0;
        foreach (var id in _items)
        {
            if (id == itemId) count++;
        }
        return count;
    }

    /// <summary>
    /// Remove all items.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }
}
