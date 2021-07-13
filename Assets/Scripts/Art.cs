using UnityEngine;

// Very small placeholder art helper. The 2021 version used plain coloured
// blocks while I concentrated on learning movement, classes and collisions.
public static class Art
{
    public const int PPU = 16;

    public class PlayerSprites
    {
        public Sprite Idle, Jump, Fall;
        public Sprite[] Run;
    }

    static Sprite Block(Color32 colour, int width = 16, int height = 16)
    {
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        var pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = colour;
        texture.SetPixels32(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height),
            new Vector2(0.5f, 0f), PPU, 0, SpriteMeshType.FullRect);
    }

    public static PlayerSprites BuildPlayer(int player)
    {
