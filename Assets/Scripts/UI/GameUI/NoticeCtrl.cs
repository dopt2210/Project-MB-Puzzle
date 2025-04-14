using System.Collections;
using TMPro;
using UnityEngine;

public class NoticeCtrl : MonoBehaviour
{
    public static NoticeCtrl Instance {  get; private set; }
    [SerializeField] private GameObject notifyImage; 
    [SerializeField] private TextMeshProUGUI textDie;   
    private Vector3 startPos;                        
    private Vector3 endPos;                           
    public static bool isNotifying { get; private set; } = false;
    private string _dieByText;

    private void Awake()
    {
        if(Instance != null) { Destroy(gameObject);return; }
        Instance = this;

        startPos = notifyImage.transform.position;  
        endPos = startPos + new Vector3(0, 50, 0);
    }
    private void Update()
    {
        //if (KillPlayer.IsDie && !isNotifying) 
        //{
        //    isNotifying = true; 
        //    notifyImage.SetActive(true);
        //    notifyImage.transform.position = startPos;
        //    textDie.SetText("You die by " + _dieByText);
        //    startNotify();
        //}
        //if (Boss1.IsBossDie && !isNotifying)
        //{
        //    isNotifying = true;
        //    notifyImage.SetActive(true);
        //    notifyImage.transform.position = startPos;
        //    textDie.SetText("You win");
        //    startNotify();

        //}
    }
    public void startNotify()
    {
        StartCoroutine(ShowNotify());
    }
    private IEnumerator ShowNotify()
    {
        float elapsedTime = 0f; 
        float duration = 2f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            notifyImage.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);

            Color textColor = textDie.color;
            textColor.a = Mathf.Lerp(1, 0, elapsedTime / duration);
            textDie.color = textColor;

            yield return null;
        }

        notifyImage.SetActive(false);
        isNotifying = false;
    }
    public void SetTextWhenDie(string text)
    {
        _dieByText = text;
    }
}
