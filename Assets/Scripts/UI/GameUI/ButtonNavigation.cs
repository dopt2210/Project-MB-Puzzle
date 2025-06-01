using UnityEngine;
using UnityEngine.UI;

public class ButtonNavigation : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] MonoBehaviour puzzleBoard;
    private IBoardButton board;
    private void Reset()
    {
        buttons = GetComponentsInChildren<Button>(true);
    }
    private void OnEnable()
    {
        board = puzzleBoard as IBoardButton;
        if (board == null)
        {
            Debug.LogError($"{puzzleBoard.name} do not implement IBoardButton", this);
        }
        AddBtnEvent();
    }
    void AddBtnEvent()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
        buttons[0].onClick.AddListener(()=> board.StartGame());
        buttons[1].onClick.AddListener(()=> board.ResetGame());
        buttons[2].onClick.AddListener(()=> board.CloseGame());
        
    }
}
public interface IBoardButton
{
    void StartGame();
    void ResetGame();
    void CloseGame();
}
