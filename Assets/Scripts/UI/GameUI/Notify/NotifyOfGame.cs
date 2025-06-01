using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotifyOfGame : MonoBehaviour, IBoardButton
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Image _image;

    public void SetElement(ItemSO data)
    {
        _image.sprite = data.icon;
        _text.SetText("You got an item: " + data.itemName);
    }
    public void SetElement(string data)
    {
        _text.SetText(data);
    }

    public void StartGame() {  }

    public void ResetGame() {  }

    public void CloseGame()
    {
        NotifyManager.isNotifying = false;
        gameObject.SetActive(false);
    }
    private void Reset()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _image = transform.GetChild(1).GetComponent<Image>();
    }
    private void OnEnable()
    {
        GameManager.Instance.SwitchOn();
    }
    private void OnDisable()
    {
        GameManager.Instance.SwitchOff();
    }
}
