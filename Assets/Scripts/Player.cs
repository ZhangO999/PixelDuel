using UnityEngine;

/// One duellist. Plain class, not a MonoBehaviour -- Game ticks it, which keeps
/// the update order obvious instead of depending on Unity's script ordering.
public class Player
{
    // ---- feel. These are the numbers worth fiddling with. ----
    const float Gravity        = 48f;
    const float MaxFall        = 24f;
    const float RunSpeed       = 6.5f;
    const float GroundAccel    = 90f;
    const float AirAccel       = 55f;
    const float GroundFriction = 75f;
    const float AirFriction    = 14f;
    const float JumpVel        = 16f;   // clears a 3-unit tier with room to spare
    const float JumpCut        = 0.45f; // released early -> chop the rise
    const float CoyoteTime     = 0.09f;
    const float JumpBuffer     = 0.10f;
    const float ShootDelay     = 0.30f;
    const float DropTime       = 0.16f;
    const float RespawnDelay   = 0.5f;
    const float InvulnTime     = 0.8f;

    public const int MaxHealth = 100;
    public const int ShotDamage = 25;   // three hits

    public readonly int Index;
    public readonly Body Body = new Body();
    public int Health, Score;
    public bool Alive = true;
    public int Facing = 1;

    readonly Art.PlayerSprites art;
    readonly Transform root;
    readonly SpriteRenderer sr, gunSr;
    readonly Game game;

    float animTime, coyote, buffer, dropTimer, cooldown, respawn, invuln;
    bool jumpHeldLast;
    Controls.Pad pad;

    static readonly Vector2 HandOffset = new Vector2(0.30f, 0.41f);

    public Player(Game game, int index, Sprite gunSprite)
    {
        this.game = game;
        Index = index;
        art = Art.BuildPlayer(index);

        root = new GameObject("Player" + (index + 1)).transform;
        sr = root.gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = art.Idle;
        sr.sortingOrder = 10;

        var gun = new GameObject("Gun");
        gun.transform.SetParent(root, false);
        gunSr = gun.AddComponent<SpriteRenderer>();
        gunSr.sprite = gunSprite;
        gunSr.sortingOrder = 11;

        Facing = index == 0 ? 1 : -1;
        Respawn();
    }

    public void Respawn()
    {
        Body.Pos = Level.Spawn(Index);
        Body.Vel = Vector2.zero;
        Health = MaxHealth;
        Alive = true;
        invuln = InvulnTime;
        cooldown = 0f;
        Facing = Index == 0 ? 1 : -1;
        root.gameObject.SetActive(true);
    }

    /// Where this player's shots go: up beats down, down only in the air, and
    /// otherwise straight ahead.
    public Vector2 Aim => new Vector2(Facing, 0f);

    public Rect Hitbox => Body.Box;
    public bool Vulnerable => Alive && invuln <= 0f;

    public void Tick(float dt, bool acceptInput)
    {
        pad = acceptInput ? Controls.Read(Index) : default;

        if (!Alive)
        {
            respawn -= dt;
            if (respawn <= 0f) Respawn();
            return;
        }

        if (invuln > 0f)
        {
            invuln -= dt;
            if (invuln <= 0f) sr.enabled = true;
        }

        Horizontal(dt);
        Jump(dt);
        Shoot(dt);

        Body.Vel.y = Mathf.Max(Body.Vel.y - Gravity * dt, -MaxFall);
        Body.DropThrough = dropTimer > 0f;
        dropTimer -= dt;
        Body.Move(dt);

        Animate(dt);
    }

    void Horizontal(float dt)
    {
        int want = pad.MoveX;
        float accel = Body.Grounded ? GroundAccel : AirAccel;
        float drag = Body.Grounded ? GroundFriction : AirFriction;

        if (want != 0)
        {
            Facing = want;
            Body.Vel.x = Mathf.MoveTowards(Body.Vel.x, want * RunSpeed, accel * dt);
        }
        else
        {
            Body.Vel.x = Mathf.MoveTowards(Body.Vel.x, 0f, drag * dt);
        }
    }

    void Jump(float dt)
