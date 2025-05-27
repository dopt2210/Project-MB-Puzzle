using UnityEngine;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance { get; private set; }
    [SerializeField] private SceneLoading loadingObject;
    private bool _isLoading;
    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(this);
        _isLoading = false;
    }
    private void Update()
    {
        StartLoading();
    }
    public void EnableLoading() => _isLoading = true;
    public void DisabelLoading() => _isLoading = false;
    private void StartLoading()
    {
        if (_isLoading)
        {
            loadingObject.gameObject.SetActive(true);
        }
        else
        {
            loadingObject.gameObject.SetActive(false);
        }
    }
}
