// Stand-ins for the new Input System package, so the ENABLE_INPUT_SYSTEM branch
// of Controls.cs gets type-checked too.
namespace UnityEngine.InputSystem
{
    public enum Key
    {
        A, D, W, S, Space, LeftShift, RightShift, Slash, RightCtrl, Enter,
        LeftArrow, RightArrow, UpArrow, DownArrow, R, Escape,
    }

    public class ButtonControl
    {
        public bool isPressed => false;
        public bool wasPressedThisFrame => false;
    }

    public class Keyboard
    {
        public static Keyboard current => null;
        public ButtonControl this[Key k] => null;
    }
}
