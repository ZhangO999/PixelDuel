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
    }

#if ENABLE_LEGACY_INPUT_MANAGER
    static KeyCode[] Old(Btn b)
    {
        switch (b)
        {
            case Btn.P1Left:  return new[] { KeyCode.A };
            case Btn.P1Right: return new[] { KeyCode.D };
            case Btn.P1Up:    return new[] { KeyCode.W };
            case Btn.P1Down:  return new[] { KeyCode.S };
            case Btn.P1Jump:  return new[] { KeyCode.LeftShift };
            case Btn.P1Shoot: return new[] { KeyCode.Space };
            case Btn.P2Left:  return new[] { KeyCode.LeftArrow };
            case Btn.P2Right: return new[] { KeyCode.RightArrow };
            case Btn.P2Up:    return new[] { KeyCode.UpArrow };
            case Btn.P2Down:  return new[] { KeyCode.DownArrow };
            case Btn.P2Jump:  return new[] { KeyCode.Slash };
            case Btn.P2Shoot: return new[] { KeyCode.RightShift, KeyCode.Return };
            case Btn.Start:   return new[] { KeyCode.Space, KeyCode.Return };
            case Btn.Reset:   return new[] { KeyCode.R };
            default:          return new[] { KeyCode.Escape };
        }
    }
#endif

#if ENABLE_INPUT_SYSTEM
    static Key[] New(Btn b)
    {
