using System.Collections.Generic;
using UnityEngine;

// Original game controller. It deliberately keeps everything in one place;
// this was my first C# and OOP project and I was still learning how to divide
// responsibilities between objects.
public class Game : MonoBehaviour
{
    public const int WinScore = 3;

    static bool booted;
    Player[] players;
    readonly List<Bullet> bullets = new List<Bullet>();
    readonly List<PuffParticle> particles = new List<PuffParticle>();
    Sprite bulletSprite;
    bool playing;
    string message = "PIXEL DUEL";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (booted) return;
        booted = true;
        new GameObject("PixelDuel").AddComponent<Game>();
    }

    void Awake()
    {
        Application.targetFrameRate = 60;
        SetUpCamera();
        BuildArena();
        bulletSprite = Art.Bullet();
        var gun = Art.Gun();
        players = new[] { new Player(this, 0, gun), new Player(this, 1, gun) };
    }

    void SetUpCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cam = cameraObject.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.orthographicSize = Level.H * 0.5f + 0.5f;
        cam.transform.position = new Vector3(Level.W * 0.5f, Level.H * 0.5f, -10f);
        cam.backgroundColor = new Color32(25, 30, 50, 255);
    }

    void BuildArena()
    {
        Sprite stone = Art.Solid();
        Sprite grass = Art.Grass();
        Sprite wood = Art.Ledge();
        var root = new GameObject("Arena").transform;

        for (int row = 0; row < Level.H; row++)
            for (int col = 0; col < Level.W; col++)
            {
                Sprite sprite = null;
                if (Level.Solid(col, row))
                    sprite = row > 0 && !Level.Solid(col, row - 1) ? grass : stone;
                else if (Level.OneWay(col, row))
                    sprite = wood;
                if (sprite == null) continue;

                var tile = new GameObject("Tile " + col + "," + row);
                tile.transform.SetParent(root);
                tile.transform.position = new Vector3(col, Level.TileBottom(row), 0f);
                tile.AddComponent<SpriteRenderer>().sprite = sprite;
            }
    }

