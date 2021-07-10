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
