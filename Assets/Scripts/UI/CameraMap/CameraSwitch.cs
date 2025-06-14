using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch Instance { get; private set; }

    [SerializeField] private Camera _playerCamera;

    [SerializeField] private ObjectOrder _puzzleObj;
    [SerializeField] private ObjectOrder _inventoryObj;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _playerCamera.enabled = true;
    }
    public void SwitchPlayerCamera()
    {
        _playerCamera.enabled = true;

        GameManager.Instance.SwitchOff();

    }
    public void SwitchInventoryCamera()
    {
        _playerCamera.enabled = false;

        _inventoryObj.SetOn();
        _puzzleObj.SetOff();

        GameManager.Instance.SwitchOn();

    }
    public void SwitchPuzzleCamera()
    {
        _playerCamera.enabled = false;

        _puzzleObj.SetOn();
        _inventoryObj.SetOff();

        GameManager.Instance.SwitchOn();
    }
}

[System.Serializable]
public struct ObjectOrder
{
    public Canvas canvas;
    public Camera camera;
    public void SetOn()
    {
        canvas.enabled = true;
        camera.enabled = true;
    }
    public void SetOff()
    {
        canvas.enabled = false; 
        camera.enabled = false;
    }
}
