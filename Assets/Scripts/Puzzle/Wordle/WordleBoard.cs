using UnityEngine;
using UnityEngine.UI;

public class WordleBoard : PuzzleBoardBase<WordleSO>
{
    [Header("Refs")]
    [SerializeField] private string target;
    [SerializeField] private VirtualKeyboard keyboard;
    [SerializeField] private RowWord[] rows;

    [SerializeField] private VerticalLayoutGroup boardLayout;

    #region Process vars
    private int currentRow = 0;
    private int currentCol = 0;
    public bool IsCanInput { get; private set; } = false;
    #endregion

    private void Reset()
    {
        boardLayout = GetComponentInChildren<VerticalLayoutGroup>();
        rows = boardLayout.GetComponentsInChildren<RowWord>();
        keyboard = GetComponentInChildren<VirtualKeyboard>(true);
    }

    #region UI Function
    protected override void ResetState()
    {
        IsCanInput = false;

        ResetBoardVisual();

    }
    #endregion

    #region Check result
    protected override void BuildBoard(WordleSO levelData)
    {
        ResetBoardVisual();
        target = levelData.GetRandomWord();
        currentRow = currentCol = 0;

        IsCanInput = true;

        keyboard.Show();
    }
    public void Backspace()
    {
        if (!IsCanInput) return;
        if (currentCol == 0) return;

        currentCol--;
        rows[currentRow].cols[currentCol].Clear();
    }
    public void ClearCurrentKey()
    {
        for (int i = 0; i < currentCol; i++)
            rows[currentRow].cols[i].Clear();
        currentCol = 0;
    }
    public void InputChar(char c)
    {
        if (!IsCanInput) return;
        if (currentRow >= rows.Length) return;

        if (currentCol < RowLen)
        {
            rows[currentRow].cols[currentCol].SetLetter(c);
            currentCol++;
        }
    }
    public void EnterRow()
    {
        if (!IsCanInput) return;
        if (currentCol < RowLen) return;

        string guess = GetCurrentRowString();
        CheckRow(guess);

        // Thắng
        if (guess == target)
        {
            OnPuzzleSolved();
            return;
        }

        // Chưa thắng mà hết hàng -> thua
        if (currentRow == rows.Length - 1)
        {
            OnPuzzleUnSolved();
            return;
        }

        currentRow++;
        currentCol = 0;
    }
    private void CheckRow(string guess)
    {
        char[] resultColor = new char[guess.Length];   // G = green, Y = yellow, X = gray
        bool[] usedTarget = new bool[target.Length];

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == target[i])
            {
                resultColor[i] = 'G';
                usedTarget[i] = true;
            }
        }

        for (int i = 0; i < guess.Length; i++)
        {
            if (resultColor[i] == 'G') continue;

            for (int j = 0; j < target.Length; j++)
            {
                if (!usedTarget[j] && guess[i] == target[j])
                {
                    resultColor[i] = 'Y';
                    usedTarget[j] = true;
                    break;
                }
            }

            if (resultColor[i] == '\0')
                resultColor[i] = 'X';
        }

        for (int i = 0; i < guess.Length; i++)
        {
            rows[currentRow].cols[i].SetColor(
                resultColor[i] == 'G' ? Color.green :
                resultColor[i] == 'Y' ? new Color(1f, .8f, .1f) : // vàng nhạt
                Color.gray
            );
        }
    }
    #endregion

    #region Ultilities function
    private string GetCurrentRowString()
    {
        var kw = rows[currentRow].cols;
        System.Text.StringBuilder sb = new();
        foreach (var k in kw) sb.Append(k.Letter);
        return sb.ToString().ToLower();
    }
    private int RowLen => rows[currentRow].cols.Length;
    private void ResetBoardVisual()
    {
        foreach (var row in rows)
            foreach (var kw in row.cols)
                kw.Clear();
    }


    #endregion
}
