using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    [SerializeField] private Image loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    private float loadProgress = 0f;
    [SerializeField] private float loadTime = 2f;

    private void OnEnable()
    {
        loadProgress = 0f;
        loadingBar.fillAmount = 0f;
    }

    private void Update()
    {
        SimulateLoading();
    }

    private void SimulateLoading()
    {
        if (loadProgress < loadTime)
        {
            loadProgress += Time.deltaTime;

            float progressRatio = loadProgress / loadTime;

            loadingBar.fillAmount = progressRatio;

            if (loadingText != null)
            {
                loadingText.text = $"{(progressRatio * 100):0}%";
            }
        }
        else
        {
            SceneLoadManager.instance.DisabelLoading();
        }
    }

}