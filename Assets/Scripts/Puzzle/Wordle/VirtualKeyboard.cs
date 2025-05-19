using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    private static readonly char[] ALPHABET =
    "abcdefghijklmnopqrstuvwxyz".ToCharArray();

    [SerializeField] private WordleBoard output;

    [SerializeField] private GridLayoutGroup boardLayout; 
    [SerializeField] private Button[] buttons;
    //[SerializeField] private Button[] btnOut;
    private void Reset()
    {
        output = GetComponentInParent<WordleBoard>();
        boardLayout = GetComponentInChildren<GridLayoutGroup>();
        buttons = boardLayout.GetComponentsInChildren<Button>(true);
        AutoBindKeys();
    }
    private void Awake()
    {
        Hide();
    }
    private void OnEnable()
    {
        if (buttons == null || buttons.Length == 0)
            buttons = boardLayout.GetComponentsInChildren<Button>(true);
        AutoBindKeys();
        
    }
    private void AutoBindKeys()
    {
        int len = Mathf.Min(ALPHABET.Length, buttons.Length);
        for (int i = 0; i < len; i++)
        {
            char c = ALPHABET[i];

            var label = buttons[i].GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null) label.text = c.ToString().ToUpper();

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => output.InputChar(c));
        }
        //btnOut[0].onClick.AddListener(() => output.EnterRow());
        //btnOut[1].onClick.AddListener(() => output.Backspace());
    }
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
