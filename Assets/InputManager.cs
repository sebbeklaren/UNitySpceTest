using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Singleton reference
    public static InputManager Instance { get; private set; }

    // --- Keyboard states ---
    public bool IsShiftHeld { get; private set; }
    public bool IsCtrltHeld { get; private set; }
    public bool KeyDown_M { get; private set; }
    public bool KeyDown_F { get; private set; }
    public bool Key_W { get; private set; }
    public bool Key_A { get; private set; }
    public bool Key_S { get; private set; }
    public bool Key_D { get; private set; }
    public bool Key_Q { get; private set; }
    public bool Key_E { get; private set; }

    // --- Mouse buttons ---
    public bool LeftMouseDown { get; private set; }
    public bool LeftMouseHeld { get; private set; }
    public bool LeftMouseUp { get; private set; }

    public bool RightMouseDown { get; private set; }
    public bool RightMouseHeld { get; private set; }
    public bool RightMouseUp { get; private set; }

    // --- Mouse axes ---
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }
    public float ScrollWheel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional if you want it persisting across scenes
    }

    void Update()
    {
        // Keyboard
        IsShiftHeld = Input.GetKey(KeyCode.LeftShift);
        IsCtrltHeld = Input.GetKey(KeyCode.LeftControl);

        KeyDown_M = Input.GetKeyDown(KeyCode.M);
        KeyDown_F = Input.GetKeyDown(KeyCode.F);

        Key_W = Input.GetKey(KeyCode.W);
        Key_A = Input.GetKey(KeyCode.A);
        Key_S = Input.GetKey(KeyCode.S);
        Key_D = Input.GetKey(KeyCode.D);
        Key_Q = Input.GetKey(KeyCode.Q);
        Key_E = Input.GetKey(KeyCode.E);

        // Mouse buttons
        LeftMouseDown = Input.GetMouseButtonDown(0);
        LeftMouseHeld = Input.GetMouseButton(0);
        LeftMouseUp = Input.GetMouseButtonUp(0);

        RightMouseDown = Input.GetMouseButtonDown(1);
        RightMouseHeld = Input.GetMouseButton(1);
        RightMouseUp = Input.GetMouseButtonUp(1);

        // Mouse axes
        MouseX = Input.GetAxis("Mouse X");
        MouseY = Input.GetAxis("Mouse Y");
        ScrollWheel = Input.GetAxis("Mouse ScrollWheel");
    }
}
