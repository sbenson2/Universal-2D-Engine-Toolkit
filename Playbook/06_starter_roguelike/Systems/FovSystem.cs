using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using MyGame.Roguelike.Components;
using MyGame.Roguelike.Map;
using MyGame.Roguelike.Tags;

namespace MyGame.Roguelike.Systems;

/// <summary>
/// Runs recursive shadowcast FOV for the player entity.
/// Updates <see cref="GameMap.Visible"/> and <see cref="GameMap.Explored"/> state,
/// and the player's <see cref="FieldOfView"/> component.
/// See G54_fog_of_war.md for the algorithm.
/// </summary>
public sealed class FovSystem
{
    private static readonly QueryDescription PlayerQuery =
        new QueryDescription().WithAll<GridPosition, FieldOfView, PlayerTag>();

    /// <summary>
    /// Recalculate the player's field of view.
    /// </summary>
    public void Update(World world, GameMap map)
    {
        map.ClearVisible();

        world.Query(in PlayerQuery, (Entity entity, ref GridPosition pos, ref FieldOfView fov) =>
        {
            ShadowcastFOV.Compute(map, pos.X, pos.Y, fov.Radius, fov.VisibleTiles);
        });
    }
}
