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

    public static int W => Map[0].Length;
    public static int H => Map.Length;

    public static char At(int col, int row) =>
        (col < 0 || col >= W || row < 0 || row >= H) ? '#' : Map[row][col];

    public static bool Solid(int col, int row) => At(col, row) == '#';
    public static bool OneWay(int col, int row) => At(col, row) == '=';

    public static int ColAt(float x) => Mathf.FloorToInt(x);
    public static int RowAt(float y) => H - 1 - Mathf.FloorToInt(y);
    public static float TileTop(int row) => H - row;
    public static float TileBottom(int row) => H - 1 - row;

    public static Vector2 Spawn(int player)
    {
        char want = player == 0 ? '1' : '2';
        for (int r = 0; r < H; r++)
            for (int c = 0; c < W; c++)
                if (Map[r][c] == want)
                    return new Vector2(c + 0.5f, TileBottom(r));
        return new Vector2(W * 0.5f, H * 0.5f);
    }

    /// True if a solid tile overlaps this world-space box.
    public static bool OverlapsSolid(Rect b)
    {
        const float e = 1e-4f;
        for (int r = RowAt(b.yMax - e); r <= RowAt(b.yMin + e); r++)
            for (int c = ColAt(b.xMin + e); c <= ColAt(b.xMax - e); c++)
                if (Solid(c, r)) return true;
        return false;
    }
}
