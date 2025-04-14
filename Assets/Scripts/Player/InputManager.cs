using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }
    public static PlayerInput InputPlayer { get; private set; }

    private InputAction _move;
    private InputAction _detail;
    private InputAction _interact;
    private InputAction _openMap;
    private InputAction _openItem;
    private InputAction _openDebug;
    private InputAction _openMouse;
    private InputAction _openPause;
    private InputAction _closePause;
    public Vector2 Move { get; private set; }
    public bool kOpenMap { get; private set; }
    public bool kOpenItem { get; private set; }
    public bool kOpenDebug { get; private set; }
    public bool kOpenMouse { get; private set; }
    public bool kOpenPause { get; private set; }
    public bool kClosePause { get; private set; }
    public bool kInteract { get; private set; }
    public bool kDetail { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        InputPlayer = GetComponent<PlayerInput>();

        _move = InputPlayer.actions["Move"];
        _interact = InputPlayer.actions["Interact"];
        _openItem = InputPlayer.actions["OpenItem"];
        _openDebug = InputPlayer.actions["OpenDebug"];
        _openMap = InputPlayer.actions["OpenMap"];
        _openMouse = InputPlayer.actions["OpenMouse"];
        _detail = InputPlayer.actions["Detail"];
        _openPause = InputPlayer.actions["OpenPause"];
        _closePause = InputPlayer.actions["ClosePause"];
    }

    void Update()
    {
        this.GatherInput();

    }
    protected virtual void GatherInput()
    {
        Move = _move.ReadValue<Vector2>();
        kInteract = _interact.WasPressedThisFrame();
        kDetail = _detail.WasPressedThisFrame();
        kOpenItem = _openItem.WasPressedThisFrame();
        kOpenDebug = _openDebug.WasPressedThisFrame();
        kOpenMap = _openMap.WasPressedThisFrame();
        kOpenMouse = _openMouse.WasPressedThisFrame();
        kOpenPause = _openPause.IsPressed();
        kClosePause = _closePause.IsPressed();

    }

    public static void DeactivatePlayerCtrl()
    {
        InputPlayer.currentActionMap.Disable();
    }

    public static void ActivatePlayerCtrl()
    {
        InputPlayer.currentActionMap.Enable();
    }
}
