using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// Keyboard input that works whether the project is set to the old Input
/// Manager, the new Input System package, or both. The #if symbols are defined
/// by Unity itself, so whichever setting the project ends up on, this compiles
/// and runs without anyone touching Project Settings.
public static class Controls
{
    public enum Btn
    {
        P1Left, P1Right, P1Up, P1Down, P1Jump, P1Shoot,
        P2Left, P2Right, P2Up, P2Down, P2Jump, P2Shoot,
        Start, Reset, Quit,
