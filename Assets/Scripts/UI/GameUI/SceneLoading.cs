using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    [SerializeField] private Image _loadingBar;
    [SerializeField] private TextMeshProUGUI _loadingText;


    public void UpdateProgress(float ratio)
    {
        if (_loadingBar != null)
            _loadingBar.fillAmount = ratio;

        if (_loadingText != null)
            _loadingText.text = $"{(ratio * 100f):0}%";
    }

}