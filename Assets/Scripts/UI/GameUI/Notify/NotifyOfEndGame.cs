using UnityEngine;

public class NotifyOfEndGame : MonoBehaviour, IBoardButton
{
    public void CloseGame()
    {
        gameObject.SetActive(false);   
    }

    public void ResetGame()
    {
    }

    public void StartGame()
    {
        CloseGame();
        UIHandler.Instance.BackToMainMenu();
    }
}
