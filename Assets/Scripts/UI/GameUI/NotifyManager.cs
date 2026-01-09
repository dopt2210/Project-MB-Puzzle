using System.Collections;
using UnityEngine;

public class NotifyManager : MonoBehaviour
{
    public static NotifyManager Instance {  get; private set; }
    [SerializeField] private NotifyOfOption nChoice;
    [SerializeField] private NotifyOfGame nGame;
    [SerializeField] private NotifyOfGM nGM;

    private Vector3 startPos, endPos;
    public static bool isNotifying { get; set; } = false;
    private void Reset()
    {
        nChoice = GetComponentInChildren<NotifyOfOption>(true);
        nGame = GetComponentInChildren<NotifyOfGame>(true);
        nGM = GetComponentInChildren<NotifyOfGM>(true);
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        startPos = transform.GetChild(0).position;
        endPos = new Vector3(0, 50, 0) + startPos;
    }
    //notify with item data
    public void StartNotify(ItemSO data)
    {
        isNotifying = true;

        nGame.gameObject.SetActive(true);
        nGame.SetElement(data);
    }
    //notify with only text
    public void Notify(string text)
    {
        isNotifying = true;

        nGM.gameObject.SetActive(true);
        nGM.SetElement(text);

        StartCoroutine(ShowNotify());
    }
    //notify stop with choice
    public void StartNotifyChoice(string text)
    {
        isNotifying = true;
        if(GameManager.Instance.CurrentLevel >= GameManager.Instance.MazeCount() - 1)
        {
            UIHandler.Instance.BackToMainMenu();
            return;
        }
        nChoice.gameObject.SetActive(true);
        nChoice.SetElement(text);
    } 
    private IEnumerator ShowNotify()
    {
        float elapsedTime = 0f;
        float duration = 2f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            nGame.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);

            yield return null;
        }
        nGM.gameObject.SetActive(false);
        isNotifying = false;
    }
}
