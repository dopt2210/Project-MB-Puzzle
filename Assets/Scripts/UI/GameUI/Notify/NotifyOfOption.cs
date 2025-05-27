using TMPro;
using UnityEngine;

public class NotifyOfOption : MonoBehaviour, IBoardButton
{
    [SerializeField] public TextMeshProUGUI _text;

    public void StartGame()
    {
        CloseGame();
        GameManager.Instance.LevelUpgrade();
    }
    public void ResetGame() { }
    public void CloseGame()
    {
        NotifyManager.isNotifying = false;
        gameObject.SetActive(false);
    }
    public void SetElement(string data)
    {
        _text.SetText(data);
    }

    private void Reset()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        UIHandler.Instance.PauseGame();
    }
    private void OnDisable()
    {
        UIHandler.Instance.ResumeGame();
    }
}
