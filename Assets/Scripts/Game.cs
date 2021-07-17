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
