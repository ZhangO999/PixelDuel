using System.Collections.Generic;
using UnityEngine;

/// All artwork lives here as character grids -- one char is one pixel.
/// Nothing is loaded from disk, so the game is just a folder of .cs files and
/// each player is a palette swap rather than a duplicated set of sprites.
///
///   .  transparent   s  skin         c  shirt (team)   b  boot
///   o  outline       d  skin shadow  v  shirt shadow   m  metal
///   k  pupil         h  hair         p  trousers       n  metal dark
///   w  eye white     g  hair light   q  trouser shade  r  muzzle flash
///   1-3 stone light/mid/dark   4-5 grass/grass dark   6-7 wood light/dark
public static class Art
{
    public const int PPU = 16;   // pixels per world unit: a 16x16 sprite is 1 unit

    static Color32 Hex(uint v) =>
        new Color32((byte)(v >> 16), (byte)(v >> 8), (byte)v, 255);

    static readonly Dictionary<char, Color32> Base = new Dictionary<char, Color32>
    {
        { 'o', Hex(0x191627) }, { 'k', Hex(0x0d0b16) }, { 'w', Hex(0xf2f0e5) },
        { 's', Hex(0xf3c99a) }, { 'd', Hex(0xcb9a68) },
        { 'h', Hex(0x6d3f22) }, { 'g', Hex(0x93602f) },
        { 'c', Hex(0xd4453f) }, { 'v', Hex(0x93262b) },
        { 'p', Hex(0x3c4a6b) }, { 'q', Hex(0x27324d) },
        { 'b', Hex(0x4a3323) },
        { 'm', Hex(0xa3adb8) }, { 'n', Hex(0x5c6570) },
        { 'r', Hex(0xffe8a3) },
        { '1', Hex(0x6b7a99) }, { '2', Hex(0x4a5878) }, { '3', Hex(0x2b3550) },
        { '4', Hex(0x5fae4a) }, { '5', Hex(0x3f8236) },
        { '6', Hex(0x9c6b3f) }, { '7', Hex(0x6d4526) },
    };

    /// Per-player recolour. Only the listed chars differ between the two players.
    public static Dictionary<char, Color32> Palette(int player)
    {
        var p = new Dictionary<char, Color32>(Base);
        if (player == 0)
        {
            p['c'] = Hex(0xd4453f); p['v'] = Hex(0x93262b);   // red shirt
            p['h'] = Hex(0x6d3f22); p['g'] = Hex(0x93602f);   // brown hair
            p['p'] = Hex(0x3c4a6b); p['q'] = Hex(0x27324d);
            p['b'] = Hex(0x4a3323);
        }
        else
        {
            p['c'] = Hex(0x3f8fd4); p['v'] = Hex(0x275794);   // blue shirt
            p['h'] = Hex(0xd8b25c); p['g'] = Hex(0xf2dd97);   // blonde hair
            p['p'] = Hex(0x4c4a5e); p['q'] = Hex(0x333243);
            p['b'] = Hex(0x3a3730);
        }
        return p;
    }

    // ---- character ------------------------------------------------------
    // Rows 0-12 are shared by every pose; only the three leg rows change. The
    // gun arm stays extended in all of them, which is what a shooter wants --
    // no arm swing to fight with the aim direction.

    static readonly string[] Torso =
    {
        "................",
        "...oooooooo.....",
        "..ohhhhhhhho....",
        "..ohhhhhhhho....",
        "..ohhgssssso....",
        "..ohhsswksso....",
        "..ohhsssssso....",
        "...osssdsso.....",
        "...occccccco....",
        "...ovccccccsso..",
        "...ovcccccco....",
        "...opppppppo....",
        "...opppppppo....",
    };

    static readonly string[] LegsIdle =
    {
        "...oppooppo.....",
        "..opp....ppo....",
        ".obbbo...obbbo..",
    };

    static readonly string[] LegsRun0 =
    {
        "...oppooppo.....",
        "..oppo..oppo....",
        ".obbbo..obbbo...",
    };

    static readonly string[] LegsRun1 =
    {
        "...oppooppo.....",
        "....oppppo......",
        "...obbbbo.......",
    };

    static readonly string[] LegsRun2 =
    {
        "...oppoopo......",
        "..opp...oppo....",
        ".obbbo..obbbo...",
    };

    static readonly string[] LegsRun3 =
    {
        "....oppppo......",
        "....oppppo......",
        "...obbbbbo......",
    };

    static readonly string[] LegsJump =
    {
        "...oppoopo......",
        "..obbo..oppo....",
        "..obo....obbo...",
    };

    static readonly string[] LegsFall =
    {
        "...oppoopo......",
        "..opp....ppo....",
        ".obbo.....obbo..",
    };

    static readonly string[] GunArt =
    {
        "..mmmmm.",
        "onmmmmmo",
        ".onnnno.",
        "..oo....",
    };

    static readonly string[] BulletArt =
    {
        ".rr.",
        "rmmr",
        ".rr.",
    };

    // ---- terrain --------------------------------------------------------

    static readonly string[] TileSolid =
    {
        "1111111111111111",
        "1222222222222221",
        "1222232222222221",
        "1222222222232221",
        "1222222222222221",
        "1223222222222221",
        "1222222222222221",
        "1222222223222221",
        "1222322222222221",
        "1222222222222221",
        "1222222222222321",
        "1222222322222221",
        "1222222222222221",
        "1232222222222221",
        "1222222222222221",
        "1333333333333331",
    };

    static readonly string[] TileGrass =
    {
        "4444444444444444",
        "4544454444544454",
        "5555555555555555",
        "1222222222222221",
        "1222232222222221",
        "1222222222232221",
        "1222222222222221",
        "1223222222222221",
        "1222222222222221",
        "1222222223222221",
        "1222322222222221",
        "1222222222222221",
        "1222222222222321",
        "1222222322222221",
        "1222222222222221",
        "1333333333333331",
    };

    static readonly string[] TileLedge =
    {
        "6666666666666666",
        "6766767766767676",
        "7777777777777777",
        "3333333333333333",
        "................",
        "................",
        "................",
        "................",
        "................",
        "................",
        "................",
        "................",
        "................",
        "................",
        "................",
        "................",
    };

    // ---- building actual Sprites ---------------------------------------

    static Sprite Make(string[] rows, Dictionary<char, Color32> pal, Vector2 pivot)
    {
        int w = rows[0].Length, h = rows.Length;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,      // keep the pixels crisp
            wrapMode = TextureWrapMode.Clamp,
        };
        var px = new Color32[w * h];
        var clear = new Color32(0, 0, 0, 0);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                // string rows read top-down, texture rows read bottom-up
                char ch = rows[h - 1 - y][x];
                px[y * w + x] = ch == '.' ? clear : pal[ch];
            }
        tex.SetPixels32(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), pivot, PPU, 0,
                             SpriteMeshType.FullRect);
    }

    /// Every sprite one player needs, built once at startup.
    public class PlayerSprites
    {
        public Sprite Idle, Jump, Fall;
        public Sprite[] Run;
    }

    static readonly Vector2 FeetPivot = new Vector2(0.5f, 0f);
    static readonly Vector2 Mid = new Vector2(0.5f, 0.5f);

    public static PlayerSprites BuildPlayer(int player)
    {
        var pal = Palette(player);
        string[] Pose(string[] legs)
        {
            var rows = new string[Torso.Length + legs.Length];
            Torso.CopyTo(rows, 0);
            legs.CopyTo(rows, Torso.Length);
            return rows;
        }
        return new PlayerSprites
        {
            Idle = Make(Pose(LegsIdle), pal, FeetPivot),
            Jump = Make(Pose(LegsJump), pal, FeetPivot),
            Fall = Make(Pose(LegsFall), pal, FeetPivot),
            Run = new[]
            {
                Make(Pose(LegsRun0), pal, FeetPivot),
                Make(Pose(LegsRun1), pal, FeetPivot),
                Make(Pose(LegsRun2), pal, FeetPivot),
                Make(Pose(LegsRun3), pal, FeetPivot),
            },
        };
    }

    // Pivot the gun at its grip so rotating it to aim up/down stays put.
    public static Sprite Gun() => Make(GunArt, Base, new Vector2(0.1f, 0.4f));
    public static Sprite Bullet() => Make(BulletArt, Base, Mid);
    public static Sprite Solid() => Make(TileSolid, Base, Vector2.zero);
    public static Sprite Grass() => Make(TileGrass, Base, Vector2.zero);
    public static Sprite Ledge() => Make(TileLedge, Base, Vector2.zero);

    /// A flat 1x1 white sprite, tinted and scaled for HUD bars and the backdrop.
    public static Sprite Solid1x1()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        { filterMode = FilterMode.Point };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Mid, 1f, 0,
                             SpriteMeshType.FullRect);
    }
}
