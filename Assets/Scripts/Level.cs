using UnityEngine;

/// The arena, as a character grid, plus the collision queries everything else
/// asks it. Row 0 is the top row. Tile (col,row) covers world x [col, col+1]
/// and y [H-1-row, H-row], so one tile is exactly one world unit.
///
///   #  solid   =  one-way ledge   .  air   1/2  player spawn
public static class Level
{
    public static readonly string[] Map =
    {
        "###################",
        "#.................#",
        "#.................#",
        "#.................#",
        "#..====.....====..#",   // upper tier, surface y = 7
        "#.................#",
        "#.................#",
        "#....===...===....#",   // lower tier, split so you can drop through
        "#.................#",
        "#.1.............2.#",
        "###################",   // floor,      surface y = 1
    };
