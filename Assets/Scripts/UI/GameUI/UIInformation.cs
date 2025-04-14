using TMPro;
using UnityEngine;

public class UIInformation : MonoBehaviour
{
    private static UIInformation instance;
    public static UIInformation Instance {  get { return instance; } }

    [SerializeField] private TextMeshProUGUI[] texts;

    [SerializeField] private bool autoUpdateTime = true;

    public float timePlay = 0;
    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        LoadComponents();
    }
    private void Update()
    {
        if (autoUpdateTime)
            UpdateTime(timePlay+=Time.deltaTime);
    }
    public void UpdateLevel(int level)
    {
        if (texts[0] != null)
        {
            texts[0].text = $"Level: {level}";
        }
            
    }
    public void UpdateTime(float time)
    {
        if (texts[1] != null)
        {
            texts[1].text = $"Time: {time:F1}s";
        }
    }
    public void ResetTime(float time)
    {
        timePlay = 0;
        if (texts[1] != null)
        {
            texts[1].text = $"Time: {time:F1}s";
        }
    }
    public void UpdateNumberCount(int number)
    {
        if (texts[2] != null)
        {
            texts[2].text = $"Puzzle Count: {number}";
        }
    }
    public void ShowHint(string hint)
    {
        if (texts[3] != null)
        {
            texts[3].text = $"Hint: {hint}";
        }
    }
    private void LoadComponents()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();
    }
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

}
