using UnityEngine;

public class UIInstance : MonoBehaviour
{
    public static UIInstance Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
