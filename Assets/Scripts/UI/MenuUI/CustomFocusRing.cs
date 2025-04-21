using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class CustomFocusRing : MonoBehaviour
{
    [SerializeField] private UIDocument _document;
    [SerializeField] private VisualElement _root;
    [SerializeField] private AudioMixer audioMixer;

    List<Button> _inOptionChoice_buttons = new List<Button>();
    List<Button> _inGraphic_buttons = new List<Button>();
    List<Button> _inVolume_buttons = new List<Button>();
    List<Button> _inGuide_buttons = new List<Button>();
    List<Button> _inMenu_buttons = new List<Button>();
    List<Slider> _inVolume_sliders = new List<Slider>();

    private VisualElement _optionElement;

    private VisualElement _graphicElement;
    private VisualElement _volumeElement;
    private VisualElement _guideElement;
    private VisualElement _menuElement;

    private Dictionary<Button, VisualElement> _buttonToPanelMap = new Dictionary<Button, VisualElement>();
    private VisualElement _currentPanel;

    [SerializeField] private bool _isInPanelMode = false;
    private Button _lastFocusedNavButton;
    private void Reset()
    {
        _document = GetComponent<UIDocument>();
        audioMixer = Resources.Load<AudioMixer>("Master");
    }
    private void OnEnable()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;
        SetElement();
        SetEvent();
        SetButtonsEvent();
        //SetButtonDisplay(true);
    }
    void SetElement()
    {
        _optionElement = _root.Q<VisualElement>("OptionElement");

        _graphicElement = _optionElement.Q<VisualElement>("Graphic");
        _volumeElement = _optionElement.Q<VisualElement>("Volume");
        _guideElement = _optionElement.Q<VisualElement>("Guide");
        _menuElement = _optionElement.Q<VisualElement>("Menu");

        _inOptionChoice_buttons = _optionElement.Query<Button>().ToList();

        _inVolume_sliders = _optionElement.Query<Slider>().ToList();
        _inVolume_buttons = _volumeElement.Query<Button>().ToList();
        _inGraphic_buttons = _graphicElement.Query<Button>().ToList();
        _inGuide_buttons = _guideElement.Query<Button>().ToList();
        _inMenu_buttons = _menuElement.Query<Button>().ToList();

        _buttonToPanelMap[_inOptionChoice_buttons[0]] = _graphicElement;
        _buttonToPanelMap[_inOptionChoice_buttons[1]] = _volumeElement;
        _buttonToPanelMap[_inOptionChoice_buttons[2]] = _guideElement;
        _buttonToPanelMap[_inOptionChoice_buttons[3]] = _menuElement;

    }
    void SetEvent()
    {
        foreach(var button in _inOptionChoice_buttons)
        {
            button.clicked += ClickButton;
        }
        _inGraphic_buttons[0].clicked += () => ExitPanelMode();

        _inVolume_buttons[0].clicked += () => ExitPanelMode();

        _inGuide_buttons[0].clicked += () => ExitPanelMode();

        _inMenu_buttons[0].clicked += () => Continue();
        _inMenu_buttons[1].clicked += () => ResetGame();
        _inMenu_buttons[2].clicked += () => LoadGame();
        _inMenu_buttons[3].clicked += () => SaveGame();
        _inMenu_buttons[4].clicked += () => QuitGame();
        _inMenu_buttons[5].clicked += () => HideOptionPanel();

        _inVolume_sliders[0].RegisterValueChangedCallback(evt => UITools.UpdateMusicVolume(evt.newValue, audioMixer));
        _inVolume_sliders[1].RegisterValueChangedCallback(evt => UITools.UpdateSFXVolume(evt.newValue, audioMixer));

        SetFocusPanelEvent();

    }
    void SetButtonsEvent()
    {
        foreach (Button button in _inOptionChoice_buttons)
        {
            //button.RegisterCallback<PointerDownEvent>(UITools.OnPointerClick);
            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
            //button.RegisterCallback<PointerEnterEvent>(UITools.OnPointerEnter);
            //button.RegisterCallback<ClickEvent>(UITools.OnPointerClick);
            //button.RegisterCallback<KeyDownEvent>(UITools.OnPointerClick);
        }
    }
    void ClickButton()
    {
        SoundManager.Instance.PlaySound2D("Click");
    }
    private void SetFocusPanelEvent()
    {
        foreach (var kvp in _buttonToPanelMap)
        {

            var btn = kvp.Key;
            var panel = kvp.Value;

            btn.clicked += () =>
            {
                EnterPanelMode(btn, panel);
            };
            btn.RegisterCallback<FocusInEvent>(evt =>
            {
                ShowOnlyThisPanel(kvp.Value);

            });
            
            btn.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    EnterPanelMode(btn, panel);
                    evt.StopPropagation();
                }
            });
        }
    }

    #region panel display
    private void EnterPanelMode(Button navButton, VisualElement panel)
    {
        _isInPanelMode = true;
        _lastFocusedNavButton = navButton;
        foreach (var k in _buttonToPanelMap.Keys)
        {
            k.focusable = false;
        }
        //ShowOnlyThisPanel(panel);
        EnablePanelFocusable(panel);
        FocusFirstElementIn(panel);
    }
    private void ExitPanelMode()
    {
        _isInPanelMode = false;
        foreach (var k in _buttonToPanelMap.Keys)
        {
            k.focusable = true;
        }
        DisablePanelsFocusable(_buttonToPanelMap);
        if (_lastFocusedNavButton != null)
        {
            _lastFocusedNavButton.Focus();
        }
    }
    private void FocusFirstElementIn(VisualElement panel)
    {
        var firstFocusable = panel.Query<VisualElement>().Where(x => x.focusable).First();
        if (firstFocusable != null)
        {
            firstFocusable.Focus();
        }
    }
    private void DisablePanelsFocusable(Dictionary<Button, VisualElement> panels)
    {
        foreach (var panel in panels.Values)
        {
            foreach (var child in panel.Children())
            {
                child.focusable = false;
            }
        }
    }
    private void EnablePanelFocusable(VisualElement panel)
    {
        foreach (var child in panel.Children())
        {
            child.focusable = true;
        }
    }
    private void ShowOnlyThisPanel(VisualElement targetPanel)
    {
        DisablePanelsFocusable(_buttonToPanelMap);
        if (_currentPanel != null)
            _currentPanel.style.display = DisplayStyle.None;

        targetPanel.style.display = DisplayStyle.Flex;
        _currentPanel = targetPanel;
    }
    private void SetButtonDisplay(bool isPlay)
    {
        if (isPlay == true) return;

        foreach (var btn in _inMenu_buttons)
        {
            if (btn.name == "backToNavBtn") continue;
            btn.style.display = DisplayStyle.None;
        }

    }
    #endregion

    #region panel action
    public void Show()
    {
        gameObject.SetActive(true);
        InputManager.InputPlayer.SwitchCurrentActionMap("UI");


        // Lấy phần tử muốn focus (ví dụ: nút đầu tiên trong menu)
        var firstButton = _root.Q<Button>("TabGraphic");

        if (firstButton != null)
        {
            // Dùng schedule để delay focus sau khi layout hoàn tất
            _root.schedule.Execute(() =>
            {
                firstButton.Focus();
            }).ExecuteLater(1); // Chờ 1 frame
        }

    } 
    public void Hide()
    {
        gameObject.SetActive(false);
        InputManager.InputPlayer.SwitchCurrentActionMap("Player");
    }
    public void ShowOptionPanel()
    {
        gameObject.SetActive(true);
    }
    public void HideOptionPanel()
    {
        foreach (var v in _buttonToPanelMap.Values)
        {
            v.style.display = DisplayStyle.None;
        }
        ExitPanelMode();
    }
    private void Continue()
    {
        Debug.Log("Continue");
    }
    private void ResetGame()
    {
        Debug.Log("ResetGame");
    }
    private void LoadGame()
    {
        Debug.Log("ResetGame");
    }
    private void SaveGame()
    {
        Debug.Log("SaveGame");
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
