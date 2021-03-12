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
        switch (b)
        {
            case Btn.P1Left:  return new[] { Key.A };
            case Btn.P1Right: return new[] { Key.D };
            case Btn.P1Up:    return new[] { Key.W };
            case Btn.P1Down:  return new[] { Key.S };
            case Btn.P1Jump:  return new[] { Key.LeftShift };
            case Btn.P1Shoot: return new[] { Key.Space };
            case Btn.P2Left:  return new[] { Key.LeftArrow };
            case Btn.P2Right: return new[] { Key.RightArrow };
            case Btn.P2Up:    return new[] { Key.UpArrow };
            case Btn.P2Down:  return new[] { Key.DownArrow };
            case Btn.P2Jump:  return new[] { Key.Slash };
            case Btn.P2Shoot: return new[] { Key.RightShift, Key.Enter };
            case Btn.Start:   return new[] { Key.Space, Key.Enter };
            case Btn.Reset:   return new[] { Key.R };
            default:          return new[] { Key.Escape };
        }
    }
#endif

    public static bool Held(Btn b)
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            foreach (var k in New(b))
                if (kb[k].isPressed) return true;
            return false;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        foreach (var k in Old(b))
            if (Input.GetKey(k)) return true;
#endif
        return false;
    }

    public static bool Pressed(Btn b)
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            foreach (var k in New(b))
                if (kb[k].wasPressedThisFrame) return true;
            return false;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        foreach (var k in Old(b))
            if (Input.GetKeyDown(k)) return true;
#endif
        return false;
    }

