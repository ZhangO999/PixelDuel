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
        Color32 colour = player == 0
            ? new Color32(210, 65, 60, 255)
            : new Color32(60, 135, 210, 255);
        var idle = Block(colour);
        return new PlayerSprites
        {
            Idle = idle,
            Jump = idle,
            Fall = idle,
            Run = new[] { idle, idle, idle, idle },
        };
    }

    public static Sprite Gun() => Block(new Color32(150, 155, 165, 255), 8, 4);
    public static Sprite Bullet() => Block(new Color32(255, 225, 120, 255), 3, 3);
    public static Sprite Solid() => Block(new Color32(75, 85, 115, 255));
    public static Sprite Grass() => Block(new Color32(75, 125, 75, 255));
    public static Sprite Ledge() => Block(new Color32(135, 90, 50, 255));
    public static Sprite Solid1x1() => Block(Color.white, 1, 1);
}

