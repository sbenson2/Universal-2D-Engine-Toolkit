namespace MyGame.TopDown.Inventory;

/// <summary>
/// Type categories for items.
/// </summary>
public enum ItemType
{
    /// <summary>Restores HP or applies a buff.</summary>
    Consumable,
    /// <summary>Equippable weapon.</summary>
    Weapon,
    /// <summary>Equippable armor or accessory.</summary>
    Armor,
    /// <summary>Key item / quest item (not consumable or equippable).</summary>
    KeyItem,
    /// <summary>Miscellaneous / crafting material.</summary>
    Material
}

/// <summary>
/// Static definition for an item type. Immutable after creation.
/// </summary>
/// <param name="Id">Unique string ID (e.g. "potion", "iron_sword").</param>
/// <param name="Name">Display name.</param>
/// <param name="Description">Flavor/help text.</param>
/// <param name="Type">Item category.</param>
/// <param name="Value">Generic stat value — interpret based on Type (heal amount, attack bonus, etc.).</param>
public record ItemDefinition(string Id, string Name, string Description, ItemType Type, int Value);

/// <summary>
/// Static registry of all item definitions in the game.
/// Look up items by ID. Populate at startup or load from data files.
/// </summary>
public static class ItemDatabase
{
    private static readonly Dictionary<string, ItemDefinition> Items = new();

    /// <summary>
    /// Register an item definition. Call during initialization.
    /// </summary>
    public static void Register(ItemDefinition item)
    {
        Items[item.Id] = item;
    }

    /// <summary>
    /// Look up an item by ID. Returns null if not found.
    /// </summary>
    public static ItemDefinition? Get(string id)
    {
        return Items.TryGetValue(id, out var item) ? item : null;
    }

    /// <summary>
    /// Returns all registered item definitions.
    /// </summary>
    public static IEnumerable<ItemDefinition> GetAll() => Items.Values;

    /// <summary>
    /// Seeds the database with starter RPG items. Call once at game start.
    /// Replace or extend with data-driven loading for production.
    /// </summary>
    public static void SeedDefaults()
    {
        Register(new ItemDefinition("potion", "Potion", "Restores 20 HP.", ItemType.Consumable, 20));
        Register(new ItemDefinition("hi_potion", "Hi-Potion", "Restores 50 HP.", ItemType.Consumable, 50));
        Register(new ItemDefinition("iron_sword", "Iron Sword", "A sturdy iron blade.", ItemType.Weapon, 5));
        Register(new ItemDefinition("leather_armor", "Leather Armor", "Basic protective gear.", ItemType.Armor, 3));
        Register(new ItemDefinition("old_key", "Old Key", "A rusty key. Opens something.", ItemType.KeyItem, 0));
    }
}
