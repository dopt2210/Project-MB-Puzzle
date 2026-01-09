using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIOptionComponent : MonoBehaviour
{
    public float musicVolume { get; set; }
    public float sfxVolume { get; set; }

    public CanvasGroup canvasGroup;
    [SerializeField] private Transform keyBindingPanel;
    public Slider[] sliders;
    [SerializeField] private Button[] buttons;
    private void Reset()
    {
        sliders = GetComponentsInChildren<Slider>(true);
        buttons = GetComponentsInChildren<Button>(true);
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }
    private void LoadComponents()
    {
        if (sliders[0] != null)
        {
            sliders[0].onValueChanged.AddListener(value => UITools.UpdateMusicVolume(value, UIInstance.audioMixer));
        }
        if (sliders[1] != null)
        {
            sliders[1].onValueChanged.AddListener(value => UITools.UpdateSFXVolume(value, UIInstance.audioMixer));
        }
        foreach (var button in buttons)
        {
            UITools.AddEventTrigger(button.gameObject, EventTriggerType.PointerEnter, UITools.OnPointerEnter);
            UITools.AddEventTrigger(button.gameObject, EventTriggerType.PointerClick, UITools.OnPointerClick);
        }
        buttons[0].onClick.AddListener(ShowKeyBinding); // Key Binding
        buttons[2].onClick.AddListener(Return); 
        buttons[15].onClick.AddListener(HideKeyBinding); // Hide Key Binding
    }
    private void OnEnable()
    {
        LoadComponents();
    }
    private void OnDisable()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
        if (sliders[0] != null)
        {
            sliders[0].onValueChanged.RemoveAllListeners();
        }
        if (sliders[1] != null)
        {
            sliders[1].onValueChanged.RemoveAllListeners();
        }
    }
    public void ShowKeyBinding()
    {
        canvasGroup.blocksRaycasts = false;
        keyBindingPanel.gameObject.SetActive(true);
    }
    public void HideKeyBinding()
    {
        canvasGroup.blocksRaycasts = true;
        keyBindingPanel.gameObject.SetActive(false);
    }
    public void Return()
    {
        UIInstance.Instance.HideOptionUI();
    }
}
