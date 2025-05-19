using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch Instance { get; private set; }

    [SerializeField] private Camera gameplayCam;
    //[SerializeField] private Camera puzzleCam;
    //[SerializeField] private Camera inventoryCam;

    [SerializeField] private CamXCan puzzle;
    [SerializeField] private CamXCan inven;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameplayCam.enabled = true;
    }
    public void ShowGameplayCam()
    {
        gameplayCam.enabled = true;
        //game.SetOn();

        InputManager.InputPlayer.SwitchCurrentActionMap("Player");

    }
    public void ShowInventoryCam()
    {
        //inventoryCam.enabled = true;

        gameplayCam.enabled = false;

        inven.SetOn();

        puzzle.SetOff();
        InputManager.InputPlayer.SwitchCurrentActionMap("UI");

    }
    public void ShowPuzzleCam()
    {
        gameplayCam.enabled = false;

        //puzzleCam.enabled = true;
        //inventoryCam.enabled = false;
        puzzle.SetOn();

        inven.SetOff();

        InputManager.InputPlayer.SwitchCurrentActionMap("UI");
    }
}
[System.Serializable]
public struct CamXCan
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
