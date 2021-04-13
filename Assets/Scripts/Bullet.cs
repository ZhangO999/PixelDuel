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

