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
    public Vector2 Aim =>
        pad.Up ? Vector2.up :
        new Vector2(Facing, 0f);

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
    {
        coyote = Body.Grounded ? CoyoteTime : coyote - dt;
        buffer = pad.JumpDown ? JumpBuffer : buffer - dt;

        // Down + jump on a ledge drops through it instead of jumping.
        if (pad.JumpDown && pad.Down && Body.Grounded)
        {
            int r = Level.RowAt(Body.Pos.y - 0.1f);
            int c = Level.ColAt(Body.Pos.x);
            if (Level.OneWay(c, r))
            {
                dropTimer = DropTime;
                Body.Pos.y -= 0.06f;
                buffer = 0f;
                jumpHeldLast = true;
                return;
            }
        }

        if (buffer > 0f && coyote > 0f)
        {
            Body.Vel.y = JumpVel;
            Body.Grounded = false;
            buffer = 0f;
            coyote = 0f;
        }

        // Let go early and you get a shorter hop.
        if (jumpHeldLast && !pad.Jump && Body.Vel.y > 0f)
            Body.Vel.y *= JumpCut;
        jumpHeldLast = pad.Jump;
    }

    void Shoot(float dt)
    {
        cooldown -= dt;
        if (!pad.Shoot || cooldown > 0f) return;
        cooldown = ShootDelay;

        Vector2 aim = Aim;
        Vector2 hand = Body.Pos + new Vector2(HandOffset.x * Facing, HandOffset.y);
        game.SpawnBullet(hand + aim * 0.45f, aim, Index);
        Body.Vel.x -= aim.x * 1.0f;    // a nudge of recoil
    }

    public void Damage(int amount, Vector2 from)
    {
        if (!Vulnerable) return;
        Health -= amount;
        Body.Vel.x += Mathf.Sign(from.x == 0f ? Facing : from.x) * 4.5f;
        Body.Vel.y += 3.5f;
        if (Health <= 0) Die();
    }

    void Die()
    {
        Alive = false;
        Health = 0;
        respawn = RespawnDelay;
        root.gameObject.SetActive(false);
        game.Puff(Body.Pos + new Vector2(0f, 0.5f), Index);
        game.ScoreFor(1 - Index);
    }

    void Animate(float dt)
    {
        Sprite frame;
        if (!Body.Grounded)
        {
            frame = Body.Vel.y > 0f ? art.Jump : art.Fall;
        }
        else if (Mathf.Abs(Body.Vel.x) > 0.6f)
        {
            animTime += dt * Mathf.Abs(Body.Vel.x) / RunSpeed;
            frame = art.Run[Mathf.FloorToInt(animTime * 11f) & 3];
        }
        else
        {
            animTime = 0f;
            frame = art.Idle;
        }
        sr.sprite = frame;
        sr.flipX = Facing < 0;

        // Snap to the texture's pixel grid, or the art shimmers as it slides
        // between screen pixels.
        root.position = new Vector3(
            Mathf.Round(Body.Pos.x * Art.PPU) / Art.PPU,
            Mathf.Round(Body.Pos.y * Art.PPU) / Art.PPU, 0f);

        Vector2 aim = Aim;
        gunSr.flipX = Facing < 0;
        gunSr.transform.localPosition =
            new Vector3(HandOffset.x * Facing, HandOffset.y, 0f);
        float angle = aim.y > 0.5f ? 90f * Facing : aim.y < -0.5f ? -90f * Facing : 0f;
        gunSr.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
