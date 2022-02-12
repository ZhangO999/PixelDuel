using UnityEngine;

/// Bullets stop on solid walls but pass through the thin one-way ledges
public class Bullet
{
    public const float Speed = 26f;
    const float Life = 1.5f;
    const float HalfSize = 0.09f;

    public Vector2 Pos, Vel;
    public int Owner;
    public bool Live;

    readonly GameObject go;
    readonly Transform tr;

    public Bullet(Sprite sprite)
    {
        go = new GameObject("Bullet");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 12;
        tr = go.transform;
        go.SetActive(false);
    }

    public void Fire(Vector2 pos, Vector2 dir, int owner)
    {
        Pos = pos;
        Vel = dir.normalized * Speed;
        Owner = owner;
        Live = true;
        life = Life;
        go.SetActive(true);
        tr.position = pos;
        tr.rotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    float life;

    public Rect Box => new Rect(Pos.x - HalfSize, Pos.y - HalfSize,
                                HalfSize * 2f, HalfSize * 2f);

    /// Returns false once the bullet should be retired.
    public bool Tick(float dt)
    {
        if (!Live) return false;
        life -= dt;
        if (life <= 0f) { Kill(); return false; }

        // Same sub-stepping as Body: a bullet moves most of a tile per frame,
        // so checking only the end point could shoot straight through a wall.
        int steps = Mathf.Max(1, Mathf.CeilToInt(Speed * dt / Body.MaxStep));
        float sub = dt / steps;
        for (int i = 0; i < steps; i++)
        {
            Pos += Vel * sub;
            if (Level.Solid(Level.ColAt(Pos.x), Level.RowAt(Pos.y)))
            {
                tr.position = Pos;
                Kill();
                return false;
            }
        }
        tr.position = Pos;
        return true;
    }

    public void Kill()
    {
        Live = false;
        go.SetActive(false);
    }
}
