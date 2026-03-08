namespace MyGame.Roguelike.Map;

/// <summary>
/// Recursive shadowcasting field-of-view algorithm.
/// Processes the map in 8 octants, scanning outward from the origin.
/// Walls create shadow regions that are skipped, making this much faster
/// than brute-force raycasting. See G54_fog_of_war.md §3.
/// </summary>
public static class ShadowcastFOV
{
    // Multipliers for the 8 octants — transforms (col, row) into (dx, dy)
    private static readonly int[,] OctantTransform =
    {
        {  1,  0,  0,  1 },  // octant 0: E-NE
        {  0,  1,  1,  0 },  // octant 1: N-NE
        {  0, -1,  1,  0 },  // octant 2: N-NW
        { -1,  0,  0,  1 },  // octant 3: W-NW
        { -1,  0,  0, -1 },  // octant 4: W-SW
        {  0, -1, -1,  0 },  // octant 5: S-SW
        {  0,  1, -1,  0 },  // octant 6: S-SE
        {  1,  0,  0, -1 },  // octant 7: E-SE
    };

    /// <summary>
    /// Compute field of view from (<paramref name="originX"/>, <paramref name="originY"/>)
    /// with the given <paramref name="radius"/>. Calls <see cref="GameMap.Reveal"/> for each
    /// visible tile.
    /// </summary>
    /// <param name="map">The game map providing opacity and reveal methods.</param>
    /// <param name="originX">Viewer X position.</param>
    /// <param name="originY">Viewer Y position.</param>
    /// <param name="radius">Vision radius in tiles.</param>
    public static void Compute(GameMap map, int originX, int originY, int radius)
    {
        map.Reveal(originX, originY);

        for (int octant = 0; octant < 8; octant++)
        {
            ScanOctant(map, originX, originY, radius, octant, 1, 1.0f, 0.0f);
        }
    }

    /// <summary>
    /// Compute FOV and write visible tiles into the provided set.
    /// </summary>
    public static void Compute(GameMap map, int originX, int originY, int radius,
        HashSet<(int, int)> visibleSet)
    {
        visibleSet.Clear();
        visibleSet.Add((originX, originY));
        map.Reveal(originX, originY);

        for (int octant = 0; octant < 8; octant++)
        {
            ScanOctant(map, originX, originY, radius, octant, 1, 1.0f, 0.0f, visibleSet);
        }
    }

    private static void ScanOctant(
        GameMap map, int ox, int oy, int radius,
        int octant, int row, float startSlope, float endSlope,
        HashSet<(int, int)>? visibleSet = null)
    {
        if (startSlope < endSlope) return;

        int r2 = radius * radius;

        int xx = OctantTransform[octant, 0];
        int xy = OctantTransform[octant, 1];
        int yx = OctantTransform[octant, 2];
        int yy = OctantTransform[octant, 3];

        for (int j = row; j <= radius; j++)
        {
            bool blocked = false;
            float newStart = startSlope;

            for (int i = -j; i <= 0; i++)
            {
                float leftSlope = (i - 0.5f) / (j + 0.5f);
                float rightSlope = (i + 0.5f) / (j - 0.5f);

                if (startSlope < rightSlope) continue;
                if (endSlope > leftSlope) break;

                int dx = i * xx + j * yx;
                int dy = i * xy + j * yy;
                int mapX = ox + dx;
                int mapY = oy + dy;

                if (dx * dx + dy * dy <= r2 && map.IsInBounds(mapX, mapY))
                {
                    map.Reveal(mapX, mapY);
                    visibleSet?.Add((mapX, mapY));
                }

                bool isOpaque = !map.IsInBounds(mapX, mapY) || map.IsOpaque(mapX, mapY);

                if (blocked)
                {
                    if (isOpaque)
                    {
                        newStart = rightSlope;
                    }
                    else
                    {
                        blocked = false;
                        startSlope = newStart;
                    }
                }
                else if (isOpaque && j < radius)
                {
                    blocked = true;
                    ScanOctant(map, ox, oy, radius, octant, j + 1, startSlope, leftSlope, visibleSet);
                    newStart = rightSlope;
                }
            }

            if (blocked) break;
        }
    }
}
