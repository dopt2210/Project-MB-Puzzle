using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }
    [SerializeField] private SceneLoading loadingObject;
    [SerializeField] private float _loadTime = 2f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    private void EnableLoading() => loadingObject.gameObject.SetActive(true);
    private void DisableLoading() => loadingObject.gameObject.SetActive(false);
    public void LoadSceneWithLoading(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    public void LoadSceneWithLoading()
    {
        StartCoroutine(SimulateLoading());
    }
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
       EnableLoading();
        yield return null;

        float elapsed = 0f;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }
        while (elapsed < _loadTime)
        {
            elapsed += Time.deltaTime;
            float progressRatio = Mathf.Clamp01(elapsed / _loadTime);
            loadingObject.UpdateProgress(progressRatio);
            yield return null;
        }

        op.allowSceneActivation = true;
        yield return null;

        DisableLoading();
    }
    private IEnumerator SimulateLoading()
    {
        EnableLoading();
        if(GameManager.Instance != null)
        {
            GameManager.Instance.SwitchOn();
        }
        float elapsed = 0f;
        while (elapsed < _loadTime)
        {
            elapsed += Time.deltaTime;
            float progressRatio = Mathf.Clamp01(elapsed / _loadTime);

            loadingObject.UpdateProgress(progressRatio);

            yield return null;
        }
        GameManager.Instance.SwitchOff();
        DisableLoading();
    }
}
