using Arch.Core;

namespace MyGame.Roguelike.Components;

/// <summary>
/// Simple inventory that holds entity references to carried items.
/// <see cref="MaxSlots"/> limits total capacity.
/// </summary>
public record struct Inventory(int MaxSlots, List<EntityReference> Items)
{
    /// <summary>Whether the inventory has room for another item.</summary>
    public readonly bool HasRoom => Items.Count < MaxSlots;

    /// <summary>Create an empty inventory with the given capacity.</summary>
    public static Inventory Create(int maxSlots) =>
        new(maxSlots, new List<EntityReference>(maxSlots));
}
