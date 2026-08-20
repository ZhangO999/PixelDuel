// Signature-only stand-ins for the Unity API, used ONLY to type-check the game
// with mcs. Never shipped to Unity -- tools/ sits outside Assets/.
using System;

namespace UnityEngine
{
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public static Vector2 zero => default(Vector2);
        public static Vector2 one => default(Vector2);
        public static Vector2 up => default(Vector2);
        public static Vector2 down => default(Vector2);
        public static Vector2 left => default(Vector2);
        public static Vector2 right => default(Vector2);
        public Vector2 normalized => default(Vector2);
        public float magnitude => 0f;
        public static Vector2 operator +(Vector2 a, Vector2 b) => default(Vector2);
        public static Vector2 operator -(Vector2 a, Vector2 b) => default(Vector2);
        public static Vector2 operator *(Vector2 a, float b) => default(Vector2);
        public static Vector2 operator *(float a, Vector2 b) => default(Vector2);
        public static Vector2 operator /(Vector2 a, float b) => default(Vector2);
        public static implicit operator Vector3(Vector2 v) => default(Vector3);
        public static implicit operator Vector2(Vector3 v) => default(Vector2);
    }

    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public static Vector3 zero => default(Vector3);
        public static Vector3 one => default(Vector3);
        public static Vector3 operator +(Vector3 a, Vector3 b) => default(Vector3);
        public static Vector3 operator -(Vector3 a, Vector3 b) => default(Vector3);
        public static Vector3 operator *(Vector3 a, float b) => default(Vector3);
    }

    public struct Rect
    {
        public float x, y, width, height;
        public Rect(float x, float y, float w, float h)
        { this.x = x; this.y = y; width = w; height = h; }
        public float xMin => x;
        public float xMax => x + width;
        public float yMin => y;
        public float yMax => y + height;
        public bool Overlaps(Rect other) => false;
    }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b, float a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static Color white => default(Color);
        public static Color black => default(Color);
        public static Color Lerp(Color a, Color b, float t) => default(Color);
        public static implicit operator Color(Color32 c) => default(Color);
    }

    public struct Color32
    {
        public byte r, g, b, a;
        public Color32(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static implicit operator Color32(Color c) => default(Color32);
    }

    public struct Quaternion
    {
        public static Quaternion identity => default(Quaternion);
        public static Quaternion Euler(float x, float y, float z) => default(Quaternion);
    }

    public static class Mathf
    {
        public const float PI = 3.14159265f;
        public const float Rad2Deg = 57.29578f;
        public const float Deg2Rad = 0.0174532924f;
        public static float Abs(float v) => 0f;
        public static int Abs(int v) => 0;
        public static float Max(float a, float b) => 0f;
        public static int Max(int a, int b) => 0;
        public static float Min(float a, float b) => 0f;
        public static int Min(int a, int b) => 0;
        public static float Clamp01(float v) => 0f;
        public static float Clamp(float v, float a, float b) => 0f;
        public static int FloorToInt(float v) => 0;
        public static int CeilToInt(float v) => 0;
        public static int RoundToInt(float v) => 0;
        public static float Round(float v) => 0f;
        public static float Floor(float v) => 0f;
        public static float Sign(float v) => 0f;
        public static float Sqrt(float v) => 0f;
        public static float Sin(float v) => 0f;
        public static float Cos(float v) => 0f;
        public static float Atan2(float y, float x) => 0f;
        public static float Lerp(float a, float b, float t) => 0f;
        public static float MoveTowards(float cur, float target, float delta) => 0f;
        public static bool Approximately(float a, float b) => false;
    }

    public static class Random
    {
        public static float value => 0f;
        public static float Range(float a, float b) => 0f;
        public static int Range(int a, int b) => 0;
    }

    public enum TextureFormat { RGBA32 }
    public enum FilterMode { Point, Bilinear, Trilinear }
    public enum TextureWrapMode { Repeat, Clamp }
    public enum SpriteMeshType { FullRect, Tight }
    public enum CameraClearFlags { Skybox, SolidColor, Depth, Nothing }
    public enum RuntimeInitializeLoadType { AfterSceneLoad, BeforeSceneLoad, SubsystemRegistration, AfterAssembliesLoaded }

    [AttributeUsage(AttributeTargets.Method)]
    public class RuntimeInitializeOnLoadMethodAttribute : Attribute
    {
        public RuntimeInitializeOnLoadMethodAttribute() { }
        public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t) { }
    }

    public class Object
    {
        public string name { get; set; }
        public static void Destroy(Object o) { }
        public static void DontDestroyOnLoad(Object o) { }
    }

    public class Texture : Object { }

    public class Texture2D : Texture
    {
        public Texture2D(int w, int h) { }
        public Texture2D(int w, int h, TextureFormat f, bool mip) { }
        public FilterMode filterMode { get; set; }
        public TextureWrapMode wrapMode { get; set; }
        public void SetPixel(int x, int y, Color c) { }
        public void SetPixels32(Color32[] px) { }
        public void Apply() { }
    }

    public class Sprite : Object
    {
        public static Sprite Create(Texture2D tex, Rect rect, Vector2 pivot) => null;
        public static Sprite Create(Texture2D tex, Rect rect, Vector2 pivot, float ppu) => null;
        public static Sprite Create(Texture2D tex, Rect rect, Vector2 pivot, float ppu,
                                    uint extrude, SpriteMeshType meshType) => null;
    }

    public class Component : Object
    {
        public Transform transform => null;
        public GameObject gameObject => null;
        public T AddComponent<T>() where T : Component => null;
        public T GetComponent<T>() where T : Component => null;
    }

    public class Behaviour : Component
    {
        public bool enabled { get; set; }
    }

    public class MonoBehaviour : Behaviour { }

    public class Transform : Component
    {
        public Vector3 position { get; set; }
        public Vector3 localPosition { get; set; }
        public Vector3 localScale { get; set; }
        public Quaternion rotation { get; set; }
        public Quaternion localRotation { get; set; }
        public void SetParent(Transform p) { }
        public void SetParent(Transform p, bool worldPositionStays) { }
    }

    public class GameObject : Object
    {
        public GameObject() { }
        public GameObject(string name) { }
        public string tag { get; set; }
        public Transform transform => null;
        public void SetActive(bool v) { }
        public T AddComponent<T>() where T : Component => null;
        public T GetComponent<T>() where T : Component => null;
    }

    public class Renderer : Component
    {
        public bool enabled { get; set; }
        public int sortingOrder { get; set; }
    }

    public class SpriteRenderer : Renderer
    {
        public Sprite sprite { get; set; }
        public Color color { get; set; }
        public bool flipX { get; set; }
        public bool flipY { get; set; }
    }

    public class Camera : Behaviour
    {
        public static Camera main => null;
        public bool orthographic { get; set; }
        public float orthographicSize { get; set; }
        public float aspect { get; set; }
        public CameraClearFlags clearFlags { get; set; }
        public Color backgroundColor { get; set; }
    }

    public static class Time
    {
        public static float deltaTime => 0f;
        public static float time => 0f;
        public static float fixedDeltaTime => 0f;
    }

    public static class Application
    {
        public static int targetFrameRate { get; set; }
        public static void Quit() { }
    }

    public enum KeyCode
    {
        A, D, W, S, R, Space, LeftShift, RightShift, Slash, RightControl,
        Return, LeftArrow, RightArrow, UpArrow, DownArrow, Escape,
    }

    public static class Input
    {
        public static bool GetKey(KeyCode k) => false;
        public static bool GetKeyDown(KeyCode k) => false;
        public static bool GetKeyUp(KeyCode k) => false;
    }

    public static class Debug
    {
        public static void Log(object o) { }
        public static void LogWarning(object o) { }
        public static void LogError(object o) { }
    }
}

namespace UnityEditor
{
    public static class EditorApplication
    {
        public static bool isPlaying { get; set; }
    }
}
