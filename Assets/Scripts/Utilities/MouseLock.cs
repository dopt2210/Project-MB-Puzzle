using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public static MouseLock Instance { get; private set; }

    public bool IsMouseFree { get; private set; } = false;

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
        if (InputManager.Instance.kOpenMouse)
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsMouseFree = false;
    }
    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsMouseFree = true;
    }
    public void AutoHandleMouseLockByPause(bool isPaused)
    {
        if (isPaused)
            UnlockMouse();
        else
            LockMouse();
    }
}