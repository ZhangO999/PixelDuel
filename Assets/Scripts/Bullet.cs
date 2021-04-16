using UnityEngine;

/// Bullets stop on solid walls but pass through the thin one-way ledges
public class Bullet
{
    public const float Speed = 18f;
    const float Life = 2.5f;
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

