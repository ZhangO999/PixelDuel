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
