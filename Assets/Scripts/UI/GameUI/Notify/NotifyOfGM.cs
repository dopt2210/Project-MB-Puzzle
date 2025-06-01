using TMPro;
using UnityEngine;

public class NotifyOfGM : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _text;

    private void Reset()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetElement(string data)
    {
        _text.SetText(data);
    }
}
