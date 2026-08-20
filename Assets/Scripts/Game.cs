using System.Collections.Generic;
using UnityEngine;

/// The whole game. Uses [RuntimeInitializeOnLoadMethod],
/// so there is nothing to wire up in the editor: open the project, press Play,
/// and the arena builds itself in whatever scene happens to be open.
public class Game : MonoBehaviour
{
    public const int WinScore = 5;

    enum State { Title, Playing, MatchOver }

    static bool booted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() => booted = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (booted) return;
        booted = true;
        new GameObject("PixelDuel").AddComponent<Game>();
    }

    State state = State.Title;
    Camera cam;
    Transform backdrop;
    Player[] players;
    Sprite pixel, gunSprite, bulletSprite;

    readonly List<Bullet> bullets = new List<Bullet>();
    readonly List<Particle> particles = new List<Particle>();

    SpriteRenderer[] barFill = new SpriteRenderer[2];
    SpriteRenderer scoreLabel, banner, hint1, hint2, hint3, panel;

    static readonly float Mid = Level.H * 0.5f;

    static readonly Color32 P1Colour = new Color32(0xd4, 0x45, 0x3f, 255);
    static readonly Color32 P2Colour = new Color32(0x3f, 0x8f, 0xd4, 255);
    static readonly Color32 Ink = new Color32(0xf2, 0xf0, 0xe5, 255);
    static readonly Color32 Dim = new Color32(0x9a, 0xa6, 0xc0, 255);

    void Awake()
    {
        Application.targetFrameRate = 60;

        pixel = Art.Solid1x1();
        gunSprite = Art.Gun();
        bulletSprite = Art.Bullet();

        SetUpCamera();
        BuildBackdrop();
        BuildArena();

        players = new[] { new Player(this, 0, gunSprite), new Player(this, 1, gunSprite) };

        BuildHud();
        EnterTitle();
    }

    // ---- scene construction --------------------------------------------

    void SetUpCamera()
    {
        cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera") { tag = "MainCamera" };
            cam = go.AddComponent<Camera>();
        }
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color32(0x16, 0x1a, 0x2e, 255);
        cam.transform.position = new Vector3(Level.W * 0.5f, Level.H * 0.5f, -10f);
        cam.transform.rotation = Quaternion.identity;
        FitCamera();
    }

    /// Always show the whole arena, whatever shape the game window is.
    void FitCamera()
    {
        float byHeight = Level.H * 0.5f;
        float byWidth = Level.W * 0.5f / Mathf.Max(cam.aspect, 0.1f);
        cam.orthographicSize = Mathf.Max(byHeight, byWidth) + 0.15f;
    }

    void BuildBackdrop()
    {
        const int h = 64;
        var tex = new Texture2D(1, h, TextureFormat.RGBA32, false)
        { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
        var top = new Color32(0x2a, 0x3a, 0x6b, 255);
        var bottom = new Color32(0x5e, 0x4a, 0x72, 255);
        for (int y = 0; y < h; y++)
            tex.SetPixel(0, y, Color.Lerp(bottom, top, y / (float)(h - 1)));
        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, 1, h), new Vector2(0.5f, 0.5f),
                                   1f, 0, SpriteMeshType.FullRect);

        var go = new GameObject("Backdrop");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = -100;
        backdrop = go.transform;
        backdrop.position = new Vector3(Level.W * 0.5f, Level.H * 0.5f, 1f);
    }

    void BuildArena()
    {
        var solid = Art.Solid();
        var grass = Art.Grass();
        var ledge = Art.Ledge();
        var root = new GameObject("Arena").transform;

        for (int r = 0; r < Level.H; r++)
            for (int c = 0; c < Level.W; c++)
            {
                Sprite s;
                if (Level.Solid(c, r))
                    // grass on any solid tile with open air directly above
                    s = (r > 0 && !Level.Solid(c, r - 1)) ? grass : solid;
                else if (Level.OneWay(c, r))
                    s = ledge;
                else
                    continue;

                var go = new GameObject("t" + c + "_" + r);
                go.transform.SetParent(root, false);
                go.transform.position = new Vector3(c, Level.TileBottom(r), 0f);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = s;
                sr.sortingOrder = 0;
            }
    }

    SpriteRenderer Quad(string name, Color32 colour, int order)
    {
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = pixel;
        sr.color = colour;
        sr.sortingOrder = order;
        return sr;
    }

    SpriteRenderer TextObj(string name, int order)
    {
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = order;
        return sr;
    }

    static void Place(SpriteRenderer sr, float cx, float cy, float w, float h)
    {
        sr.transform.position = new Vector3(cx, cy, 0f);
        sr.transform.localScale = new Vector3(w, h, 1f);
    }

    static void SetText(SpriteRenderer sr, string text, Color32 colour,
                        int scale, float cx, float cy)
    {
        sr.sprite = PixelFont.Text(text, colour, scale);
        sr.transform.position = new Vector3(cx, cy, 0f);
        sr.transform.localScale = Vector3.one;
        sr.enabled = true;
    }

    // ---- HUD -------------------------------------------------------------

    static readonly float BarY = Level.H - 0.5f;   // the ceiling row is the HUD strip
    const float BarH = 0.42f, BarW = 6.0f;
    static readonly float Bar1L = 1.2f;
    static readonly float Bar2R = Level.W - 1.2f;

    void BuildHud()
    {
        // The ceiling row doubles as the HUD strip.
        var strip = Quad("HudStrip", new Color32(0x11, 0x0f, 0x1c, 230), 18);
        Place(strip, Level.W * 0.5f, BarY, Level.W - 2f, 0.78f);

        for (int i = 0; i < 2; i++)
        {
            var back = Quad("BarBack" + i, new Color32(0x2a, 0x2b, 0x40, 255), 19);
            float cx = i == 0 ? Bar1L + BarW * 0.5f : Bar2R - BarW * 0.5f;
            Place(back, cx, BarY, BarW + 0.14f, BarH + 0.14f);
            barFill[i] = Quad("BarFill" + i, i == 0 ? P1Colour : P2Colour, 20);
        }

        // Sits behind the menu text so it reads against the arena.
        panel = Quad("Panel", new Color32(0x0d, 0x0b, 0x16, 210), 29);

        scoreLabel = TextObj("Score", 21);
        banner = TextObj("Banner", 30);
        hint1 = TextObj("Hint1", 30);
        hint2 = TextObj("Hint2", 30);
        hint3 = TextObj("Hint3", 30);
    }

    void UpdateHud()
    {
        for (int i = 0; i < 2; i++)
        {
            float frac = Mathf.Clamp01(players[i].Health / (float)Player.MaxHealth);
            float w = Mathf.Max(BarW * frac, 0.0001f);
            // P1 drains right-to-left, P2 left-to-right, so both empty outwards
            float cx = i == 0 ? Bar1L + w * 0.5f : Bar2R - w * 0.5f;
            Place(barFill[i], cx, BarY, w, BarH);
        }
        SetText(scoreLabel, players[0].Score + " - " + players[1].Score, Ink, 1,
                Level.W * 0.5f, BarY);
    }

    // ---- state -----------------------------------------------------------

    void EnterTitle()
    {
        state = State.Title;
        ClearBullets();
        players[0].Score = players[1].Score = 0;
        foreach (var p in players) p.Respawn();
        panel.enabled = true;
        Place(panel, Level.W * 0.5f, Mid + 1.1f, Level.W - 1.4f, 5.9f);
        SetText(banner, "PIXEL DUEL", Ink, 3, Level.W * 0.5f, Mid + 3.0f);
        SetText(hint1, "P1  AD MOVE  WS AIM  LSHIFT JUMP  SPACE FIRE",
                P1Colour, 1, Level.W * 0.5f, Mid + 1.2f);
        SetText(hint2, "P2  ARROWS MOVE AIM  SLASH JUMP  RSHIFT FIRE",
                P2Colour, 1, Level.W * 0.5f, Mid + 0.4f);
        SetText(hint3, "FIRST TO " + WinScore + "  -  PRESS SPACE OR ENTER",
                Dim, 1, Level.W * 0.5f, Mid - 1.0f);
    }

    void EnterPlaying()
    {
        state = State.Playing;
        ClearBullets();
        players[0].Score = players[1].Score = 0;
        foreach (var p in players) p.Respawn();
        banner.enabled = hint1.enabled = hint2.enabled = hint3.enabled = false;
        panel.enabled = false;
    }

    void EnterMatchOver(int winner)
    {
        state = State.MatchOver;
        ClearBullets();
        panel.enabled = true;
        Place(panel, Level.W * 0.5f, Mid + 1.5f, 15.5f, 4.0f);
        SetText(banner, "PLAYER " + (winner + 1) + " WINS",
                winner == 0 ? P1Colour : P2Colour, 3, Level.W * 0.5f, Mid + 2.2f);
        SetText(hint3, "PRESS SPACE OR ENTER FOR A REMATCH", Dim, 1,
                Level.W * 0.5f, Mid + 0.6f);
        hint1.enabled = hint2.enabled = false;
    }

    public void ScoreFor(int player)
    {
        players[player].Score++;
        if (players[player].Score >= WinScore) EnterMatchOver(player);
    }

    // ---- loop ------------------------------------------------------------

    void Update()
    {
        float dt = Mathf.Min(Time.deltaTime, 1f / 30f);
        FitCamera();
        SizeBackdrop();

        if (Controls.Pressed(Controls.Btn.Quit))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return;
        }

        if (state != State.Playing && Controls.Pressed(Controls.Btn.Start)) EnterPlaying();
        if (state == State.Playing && Controls.Pressed(Controls.Btn.Reset)) EnterTitle();

        bool live = state == State.Playing;
        foreach (var p in players) p.Tick(dt, live);

        TickBullets(dt);
        TickParticles(dt);
        UpdateHud();
    }

    void SizeBackdrop()
    {
        float h = cam.orthographicSize * 2f;
        backdrop.localScale = new Vector3(h * cam.aspect + 2f, (h + 2f) / 64f, 1f);
        backdrop.position = new Vector3(cam.transform.position.x,
                                        cam.transform.position.y, 1f);
    }

    void TickBullets(float dt)
    {
        for (int i = bullets.Count - 1; i >= 0; i--)
        {
            var b = bullets[i];
            if (!b.Live) continue;

            if (!b.Tick(dt))
            {
                Spark(b.Pos, 4, new Color32(0xff, 0xe8, 0xa3, 255));
                continue;
            }

            foreach (var p in players)
            {
                if (p.Index == b.Owner || !p.Vulnerable) continue;
                if (!b.Box.Overlaps(p.Hitbox)) continue;
                p.Damage(Player.ShotDamage, b.Vel);
                Spark(b.Pos, 8, new Color32(0xff, 0xd0, 0x88, 255));
                b.Kill();
                break;
            }
        }
    }

    public void SpawnBullet(Vector2 pos, Vector2 dir, int owner)
    {
        Bullet free = null;
        foreach (var b in bullets)
            if (!b.Live) { free = b; break; }
        if (free == null)
        {
            free = new Bullet(bulletSprite);
            bullets.Add(free);
        }
        free.Fire(pos, dir, owner);
    }

    void ClearBullets()
    {
        foreach (var b in bullets) b.Kill();
    }

    // ---- particles -------------------------------------------------------

    class Particle
    {
        public Vector2 Pos, Vel;
        public float Life, Max;
        public SpriteRenderer Sr;
    }

    Particle FreeParticle()
    {
        foreach (var p in particles)
            if (p.Life <= 0f) return p;
        var np = new Particle { Sr = Quad("Particle", Color.white, 13) };
        np.Sr.gameObject.SetActive(false);
        particles.Add(np);
        return np;
    }

    void Emit(Vector2 pos, Vector2 vel, Color32 colour, float life, float size)
    {
        var p = FreeParticle();
        p.Pos = pos; p.Vel = vel; p.Life = p.Max = life;
        p.Sr.color = colour;
        p.Sr.gameObject.SetActive(true);
        Place(p.Sr, pos.x, pos.y, size, size);
    }

    public void Spark(Vector2 pos, int count, Color32 colour)
    {
        for (int i = 0; i < count; i++)
        {
            float a = Random.value * Mathf.PI * 2f;
            float s = Random.Range(2f, 7f);
            Emit(pos, new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s, colour,
                 Random.Range(0.12f, 0.3f), 0.1f);
        }
    }

    /// The burst when someone goes down: their own colours, thrown outwards.
    public void Puff(Vector2 pos, int player)
    {
        var shirt = player == 0 ? P1Colour : P2Colour;
        var skin = new Color32(0xf3, 0xc9, 0x9a, 255);
        for (int i = 0; i < 22; i++)
        {
            float a = Random.value * Mathf.PI * 2f;
            float s = Random.Range(3f, 11f);
            Emit(pos, new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s,
                 i % 3 == 0 ? skin : shirt, Random.Range(0.35f, 0.8f),
                 Random.Range(0.12f, 0.22f));
        }
    }

    void TickParticles(float dt)
    {
        foreach (var p in particles)
        {
            if (p.Life <= 0f) continue;
            p.Life -= dt;
            if (p.Life <= 0f) { p.Sr.gameObject.SetActive(false); continue; }
            p.Vel.y -= 30f * dt;
            p.Pos += p.Vel * dt;
            p.Sr.transform.position = new Vector3(p.Pos.x, p.Pos.y, 0f);
            var c = p.Sr.color;
            c.a = Mathf.Clamp01(p.Life / p.Max);
            p.Sr.color = c;
        }
    }
}
