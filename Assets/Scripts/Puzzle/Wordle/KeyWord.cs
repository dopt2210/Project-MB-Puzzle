using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyWord : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _key;
    [SerializeField] private Image _img;

    private static readonly Color COLOR_EMPTY = Color.white;
    private static readonly Color COLOR_ACTIVE = Color.red;
    public char Letter {  get; private set; } = '\0';
    private void Awake()
    {
        _key = GetComponentInChildren<TextMeshProUGUI>();
        _img = GetComponentInChildren<Image>();
        _img.color = COLOR_EMPTY;
    }
    public bool IsEmpty => Letter == '\0';
    public void SetLetter(char key)
    {
        _key.text = key.ToString();
        this.Letter = key;

    }
    public void Clear()
    {
        Letter = '\0';
        _key.text = "";
        _img.color = COLOR_EMPTY;
    }
    public void SetColor(Color color) => _img.color = color;
    public void SetKeyActive(bool active)
    {
        if (!IsEmpty) return;            
        _img.color = active ? COLOR_ACTIVE : COLOR_EMPTY;
    }
}
