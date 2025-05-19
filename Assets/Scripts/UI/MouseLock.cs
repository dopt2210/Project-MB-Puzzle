using Unity.Cinemachine;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public static MouseLock Instance { get; private set; }

    public bool IsMouseFree { get; private set; } = false;
    [SerializeField] CinemachineInputAxisController cam;
    private void Reset()
    {
        cam = GetComponentInChildren<CinemachineInputAxisController>();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        LockMouse();
    }

    private void Update()
    {
        if (InputManager.Instance.Action.OpenMouse)
        {
            if (IsNotLocked)
                LockMouse();
            else if(IsLocked) UnlockMouse();
        }
    }

    public bool IsNotLocked => Cursor.lockState != CursorLockMode.Locked 
        && Cursor.visible == true
        && IsMouseFree == true;
    public bool IsLocked => Cursor.lockState == CursorLockMode.Locked
        && Cursor.visible == false
        && IsMouseFree == false;
    public void LockMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        LockCamera(true);
        IsMouseFree = false;
    }
    public void UnlockMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        LockCamera(false);
        IsMouseFree = true;
    }
    public void AutoHandleMouseLockByPause(bool isPaused)
    {
        if (isPaused)
            UnlockMouse();
        else
            LockMouse();
    }
    void LockCamera(bool value)
    {
        cam.enabled = value;
    }
}