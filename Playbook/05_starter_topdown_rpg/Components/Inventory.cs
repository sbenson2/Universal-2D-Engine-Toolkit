namespace MyGame.TopDown.Components;

/// <summary>
/// Simple inventory component. Stores item IDs that reference
/// <see cref="MyGame.TopDown.Inventory.ItemDatabase"/> definitions.
/// Attach to the player entity (or any entity that can carry items).
/// </summary>
/// <remarks>
/// Uses a <see cref="List{T}"/> for ordered slots. For large inventories,
/// consider a dictionary keyed by item ID with stack counts.
/// Because this contains a reference type, it must be a class-backed component
/// or the list must be initialized externally after entity creation.
/// </remarks>
/// <param name="Items">List of item IDs currently held.</param>
public record struct InventoryComponent(List<string> Items);
