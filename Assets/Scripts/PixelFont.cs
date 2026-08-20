using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// A 5x7 bitmap font, so the HUD matches the sprites instead of pulling in
/// TextMeshPro and an imported .ttf. Glyphs are rows of 1s and 0s.
public static class PixelFont
{
    const int GW = 5, GH = 7;

    static readonly Dictionary<char, string> G = new Dictionary<char, string>
    {
        { 'A', "01110/10001/10001/11111/10001/10001/10001" },
        { 'B', "11110/10001/10001/11110/10001/10001/11110" },
        { 'C', "01110/10001/10000/10000/10000/10001/01110" },
        { 'D', "11110/10001/10001/10001/10001/10001/11110" },
        { 'E', "11111/10000/10000/11110/10000/10000/11111" },
        { 'F', "11111/10000/10000/11110/10000/10000/10000" },
        { 'G', "01110/10001/10000/10111/10001/10001/01111" },
        { 'H', "10001/10001/10001/11111/10001/10001/10001" },
        { 'I', "11111/00100/00100/00100/00100/00100/11111" },
        { 'J', "00111/00010/00010/00010/00010/10010/01100" },
        { 'K', "10001/10010/10100/11000/10100/10010/10001" },
        { 'L', "10000/10000/10000/10000/10000/10000/11111" },
        { 'M', "10001/11011/10101/10101/10001/10001/10001" },
        { 'N', "10001/11001/10101/10011/10001/10001/10001" },
        { 'O', "01110/10001/10001/10001/10001/10001/01110" },
        { 'P', "11110/10001/10001/11110/10000/10000/10000" },
        { 'Q', "01110/10001/10001/10001/10101/10010/01101" },
        { 'R', "11110/10001/10001/11110/10100/10010/10001" },
        { 'S', "01111/10000/10000/01110/00001/00001/11110" },
        { 'T', "11111/00100/00100/00100/00100/00100/00100" },
        { 'U', "10001/10001/10001/10001/10001/10001/01110" },
        { 'V', "10001/10001/10001/10001/10001/01010/00100" },
        { 'W', "10001/10001/10001/10101/10101/11011/01010" },
        { 'X', "10001/10001/01010/00100/01010/10001/10001" },
        { 'Y', "10001/10001/01010/00100/00100/00100/00100" },
        { 'Z', "11111/00001/00010/00100/01000/10000/11111" },
        { '0', "01110/10001/10011/10101/11001/10001/01110" },
        { '1', "00100/01100/00100/00100/00100/00100/01110" },
        { '2', "01110/10001/00001/00110/01000/10000/11111" },
        { '3', "11111/00010/00100/00010/00001/10001/01110" },
        { '4', "00010/00110/01010/10010/11111/00010/00010" },
        { '5', "11111/10000/11110/00001/00001/10001/01110" },
        { '6', "00110/01000/10000/11110/10001/10001/01110" },
        { '7', "11111/00001/00010/00100/01000/01000/01000" },
        { '8', "01110/10001/10001/01110/10001/10001/01110" },
        { '9', "01110/10001/10001/01111/00001/00010/01100" },
        { ' ', "00000/00000/00000/00000/00000/00000/00000" },
        { '.', "00000/00000/00000/00000/00000/00110/00110" },
        { ':', "00000/00100/00100/00000/00100/00100/00000" },
        { '-', "00000/00000/00000/01110/00000/00000/00000" },
        { '!', "00100/00100/00100/00100/00100/00000/00100" },
        { '/', "00001/00010/00010/00100/01000/01000/10000" },
    };

    static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

    /// Builds (and caches) a sprite of the given text. `scale` fattens each
    /// font pixel into a scale x scale block so headings can be bigger without
    /// any filtering. Pivot is the centre.
    public static Sprite Text(string text, Color32 colour, int scale = 1)
    {
        text = text.ToUpperInvariant();
        string key = text + "|" + colour.r + "," + colour.g + "," + colour.b + "|" + scale;
        if (Cache.TryGetValue(key, out var cached)) return cached;

        int w = (text.Length * (GW + 1) - 1) * scale;
        int h = GH * scale;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };

        var px = new Color32[w * h];
        var clear = new Color32(0, 0, 0, 0);
        for (int i = 0; i < px.Length; i++) px[i] = clear;

        for (int i = 0; i < text.Length; i++)
        {
            if (!G.TryGetValue(text[i], out var glyph)) glyph = G[' '];
            var rows = glyph.Split('/');
            for (int gy = 0; gy < GH; gy++)
                for (int gx = 0; gx < GW; gx++)
                {
                    if (rows[gy][gx] != '1') continue;
                    int bx = (i * (GW + 1) + gx) * scale;
                    int by = (GH - 1 - gy) * scale;      // texture rows go bottom-up
                    for (int sy = 0; sy < scale; sy++)
                        for (int sx = 0; sx < scale; sx++)
                            px[(by + sy) * w + bx + sx] = colour;
                }
        }

        tex.SetPixels32(px);
        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f),
                                   Art.PPU, 0, SpriteMeshType.FullRect);
        Cache[key] = sprite;
        return sprite;
    }
}
