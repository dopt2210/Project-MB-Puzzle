using TMPro;
using UnityEngine;

public class UIInformation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] texts;

    [SerializeField] private bool autoUpdateTime = true;

    public float timePlay = 0;
    private void Reset()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();
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
            texts[0].text = $"Maze Level: {level}";
        }
    }
    public void UpdateTime(float time)
    {
        if (texts[1] != null)
        {
            texts[1].text = $"{time:F1}s";
        }
    }
    public void ResetTime()
    {
        timePlay = 0;
        if (texts[1] != null)
        {
            texts[1].text = $"{timePlay:F1}s";
        }
    }
    public void ShowHint(string hint)
    {
        if (texts[2] != null)
        {
            texts[2].text = $"{hint}";
        }
    }
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

}
