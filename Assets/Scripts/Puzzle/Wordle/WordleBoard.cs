using UnityEngine;
using UnityEngine.UI;

public class WordleBoard : MonoBehaviour, IBoardButton
{
    [Header("Refs")]
    [SerializeField] private string target;
    [SerializeField] private WordleSO levelData;
    [SerializeField] private VirtualKeyboard keyboard;

    [SerializeField] private VerticalLayoutGroup boardLayout;
    [SerializeField] private RowWord[] rows;

    #region Process vars
    private int currentRow = 0;
    private int currentCol = 0;
    public bool IsGameOver { get; private set; }
    public bool IsCanInput { get; private set; } = false;
    #endregion

    private void Reset()
    {
        boardLayout = GetComponentInChildren<VerticalLayoutGroup>();
        rows = boardLayout.GetComponentsInChildren<RowWord>();
        keyboard = GetComponentInChildren<VirtualKeyboard>(true);
    }
    private void OnEnable()
    {
        if (levelData == null)
        {
            Debug.LogWarning("You must drag level data Resources/Scriptabel/WordleSO");
            return;
        }

        if (rows.Length != levelData.rows)
            Debug.LogWarning($"Exception {rows.Length} but Required {levelData.rows}");

        target = levelData.GetRandomWord();
    }

    #region UI Function
    public void StartGame()
    {
        ResetBoardVisual();
        target = levelData.GetRandomWord();
        currentRow = currentCol = 0;

        IsCanInput = true;
        IsGameOver = false;

        keyboard.Show();  
    }
    public void ResetGame()
    {
        IsCanInput = false;
        IsGameOver = true;

        ResetBoardVisual();

        keyboard.Hide();
    }
    public void CloseGame() => transform.gameObject.SetActive(false);
    public void Backspace()
    {
        if (!IsCanInput || IsGameOver) return;
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
        if (!IsCanInput || IsGameOver) return;
        if (currentRow >= rows.Length) return;

        if (currentCol < RowLen)
        {
            rows[currentRow].cols[currentCol].SetLetter(c);
            currentCol++;
        }
    }
    public void EnterRow()
    {
        if (!IsCanInput || IsGameOver) return;
        if (currentCol < RowLen) return;

        string guess = GetCurrentRowString();
        CheckRow(guess);

        // Thắng
        if (guess == target)
        {
            Debug.Log("Win");
            IsGameOver = true;
            return;
        }

        // Chưa thắng mà hết hàng -> thua
        if (currentRow == rows.Length - 1)
        {
            Debug.Log("Lose");
            IsGameOver = true;
            return;
        }

        currentRow++;
        currentCol = 0;
    }
    #endregion

    #region Check result
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
    private void UpdateActiveCell()
    {
        foreach (var kw in rows[currentRow].cols)
            kw.SetKeyActive(false);

        if (currentCol < RowLen)
            rows[currentRow].cols[currentCol].SetKeyActive(true);
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
