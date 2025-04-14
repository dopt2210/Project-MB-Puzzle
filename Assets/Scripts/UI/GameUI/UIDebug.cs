using TMPro;
using UnityEngine;

public class UIDebug : MonoBehaviour
{
    private static UIDebug instance;
    public static UIDebug Instance {  get { return instance; } }
    [SerializeField] private TextMeshProUGUI seedText;
    [SerializeField] private TextMeshProUGUI algorithmText;
    [SerializeField] private TextMeshProUGUI[] coordText;
    [SerializeField] private TMP_InputField[] sizeOfMaze;

    [SerializeField] private MazeSO mazeSO;
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        UpdateSize();
    }
    public void UpdateCoord(Cell cell)
    {
        if (coordText[0] != null && coordText[1] != null && coordText[2] != null)
        {
            coordText[0].text = $"{cell.x}";
            coordText[1].text = $"{cell.y}";
            coordText[2].text = $"{cell.z}";
        }
            
    }
    public void UpdateSize()
    {
        sizeOfMaze[0].onEndEdit.AddListener(UpdateWidth);
        sizeOfMaze[1].onEndEdit.AddListener(UpdateHeight);
        sizeOfMaze[2].onEndEdit.AddListener(UpdateDepth);

        sizeOfMaze[0].text = mazeSO.Width.ToString();
        sizeOfMaze[1].text = mazeSO.Height.ToString();
        sizeOfMaze[2].text = mazeSO.Depth.ToString();
    }
    public void UpdateAlgo(string algo)
    {
        if (algorithmText != null)
            algorithmText.text = $"{algo}";
    }
    public void UpdateSeed(string seed)
    {
        if (seedText != null)
            seedText.text = $"Seed: {seed}";
    }

    private void UpdateWidth(string value)
    {
        if (int.TryParse(value, out int newWidth))
        {
            if (newWidth < 1) return;
            mazeSO.Width = newWidth;

        }
    }
    private void UpdateDepth(string value)
    {
        if (int.TryParse(value, out int newDepth))
        {
            if (newDepth < 1) return;
            mazeSO.Depth = newDepth;
        }
    }
    private void UpdateHeight(string value)
    {
        if (int.TryParse(value, out int newHeight))
        {
            if (newHeight < 1) return;
            mazeSO.Height = newHeight;
        }
    }
}
