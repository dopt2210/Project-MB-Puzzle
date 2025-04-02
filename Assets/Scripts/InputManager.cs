using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }
    public static PlayerInput InputPlayer { get; private set; }

    private InputAction _move;
    public Vector2 Move { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        InputPlayer = GetComponent<PlayerInput>();

        _move = InputPlayer.actions["Move"];

    }

    void Update()
    {
        this.GatherInput();

    }
    protected virtual void GatherInput()
    {
        Move = _move.ReadValue<Vector2>();
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
