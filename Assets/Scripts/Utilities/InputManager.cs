using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-200)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public static PlayerInput InputPlayer { get; private set; }

    public PlayerMovementInput Movement { get; private set; }
    public PlayerActionInput Action { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        InputPlayer = GetComponent<PlayerInput>();

        Movement = new PlayerMovementInput(InputPlayer);
        Action = new PlayerActionInput(InputPlayer);
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
public class PlayerMovementInput
{
    private readonly InputAction _move;
    private readonly InputAction _look;
    private readonly InputAction _run;

    public Vector2 Move => _move.ReadValue<Vector2>();
    public Vector2 Look => _look.ReadValue<Vector2>();
    public bool IsRunning => _run.IsPressed();

    public PlayerMovementInput(PlayerInput playerInput)
    {
        _move = playerInput.actions["Move"];
        _look = playerInput.actions["Look"];
        _run = playerInput.actions["Run"];
    }
}
public class PlayerActionInput
{
    private readonly InputAction _interact;
    private readonly InputAction _openPause;
    private readonly InputAction _closePause;
    private readonly InputAction _openMap;
    private readonly InputAction _openDebug;
    private readonly InputAction _openItem;
    private readonly InputAction _closeItem;
    private readonly InputAction _openMouse;

    public bool Interact => _interact.WasPressedThisFrame();
    public bool Pause => _openPause.WasPressedThisFrame();
    public bool Resume => _closePause.WasPressedThisFrame();
    public bool OpenMap => _openMap.WasPressedThisFrame();
    public bool OpenDebug => _openDebug.WasPressedThisFrame();
    public bool OpenItem => _openItem.WasPressedThisFrame();
    public bool CloseItem => _closeItem.WasPressedThisFrame();
    public bool OpenMouse => _openMouse.WasPressedThisFrame();

    public PlayerActionInput(PlayerInput playerInput)
    {
        _interact = playerInput.actions["Interact"];
        _openPause = playerInput.actions["OpenPause"];
        _closePause = playerInput.actions["ClosePause"];
        _openMap = playerInput.actions["OpenMap"];
        _openDebug = playerInput.actions["OpenDebug"];
        _openMouse = playerInput.actions["OpenMouse"];
        _openItem = playerInput.actions["OpenItem"];
        _closeItem = playerInput.actions["CloseItem"];
    }
}
