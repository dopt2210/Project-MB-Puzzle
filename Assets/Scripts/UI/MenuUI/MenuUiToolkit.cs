using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuUiToolkit : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private UIDocument _document;
    [SerializeField] private VisualTreeAsset _optionPanelAsset;

    List<Button> _inLeftContent_buttons = new List<Button>();
    List<Button> _inStartChoice_buttons = new List<Button>();
    List<Button> _inOptionChoice_buttons = new List<Button>();
    List<Button> _inGraphic_buttons = new List<Button>();
    List<Button> _inVolume_buttons = new List<Button>();
    List<Button> _inGuide_buttons = new List<Button>();
    List<Button> _inMenu_buttons = new List<Button>();
    List<Slider> _inVolume_sliders = new List<Slider>();

    private VisualElement _root, _container;

    private VisualElement _leftContentElement;

    private VisualElement _startElement;
    private VisualElement _optionElement;

    private VisualElement _graphicElement;
    private VisualElement _volumeElement;
    private VisualElement _guideElement;
    private VisualElement _menuElement;

    private Dictionary<Button, VisualElement> _buttonToPanelMap = new Dictionary<Button, VisualElement>();
    private VisualElement _currentPanel;

    [SerializeField] private bool _isInPanelMode = false;
    private Button _lastFocusedNavButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;

        SetElement();
        SetEvent();
        SetButtonsEvent();

    }
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Start");
        LoadData();
        SetButtonDisplay(false);
    }
    private void Update()
    {

    }
    #region button setup
    void SetButtonsEvent()
    {
        foreach (Button button in _inLeftContent_buttons)
        {
            button.RegisterCallback<PointerDownEvent>(UITools.OnPointerClick);
            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
            button.RegisterCallback<PointerEnterEvent>(UITools.OnPointerEnter);
        }
        foreach (Button button in _inStartChoice_buttons)
        {
            button.RegisterCallback<PointerDownEvent>(UITools.OnPointerClick);
            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
            button.RegisterCallback<PointerEnterEvent>(UITools.OnPointerEnter);
        }
        foreach (Button button in _inOptionChoice_buttons)
        {
            button.RegisterCallback<PointerDownEvent>(UITools.OnPointerClick);
            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
            button.RegisterCallback<PointerEnterEvent>(UITools.OnPointerEnter);
        }
    }
    void SetElement()
    {
        _container = _root.Q<VisualElement>("Container");

        _leftContentElement = _root.Q<VisualElement>("LeftContent");
        _startElement = _root.Q<VisualElement>("StartChoice");
        _optionElement = _optionPanelAsset.CloneTree();

        _graphicElement = _optionElement.Q<VisualElement>("Graphic");
        _volumeElement = _optionElement.Q<VisualElement>("Volume");
        _guideElement = _optionElement.Q<VisualElement>("Guide");
        _menuElement = _optionElement.Q<VisualElement>("Menu");

        _inLeftContent_buttons = _leftContentElement.Query<Button>().ToList();
        _inStartChoice_buttons = _startElement.Query<Button>().ToList();
        _inOptionChoice_buttons = _optionElement.Query<Button>().ToList();

        _inVolume_buttons = _volumeElement.Query<Button>().ToList();
        _inGraphic_buttons = _graphicElement.Query<Button>().ToList();
        _inGuide_buttons = _guideElement.Query<Button>().ToList();
        _inMenu_buttons = _menuElement.Query<Button>().ToList();

        _inVolume_sliders = _optionElement.Query<Slider>().ToList();

        _buttonToPanelMap[_inOptionChoice_buttons[0]] = _graphicElement;
        _buttonToPanelMap[_inOptionChoice_buttons[1]] = _volumeElement;
        _buttonToPanelMap[_inOptionChoice_buttons[2]] = _guideElement;
        _buttonToPanelMap[_inOptionChoice_buttons[3]] = _menuElement;

    }
    void SetEvent()
    {
        _inLeftContent_buttons[0].clicked += () 
            => StartCoroutine(UITools.DelayedAction(() => ClearContent(_inStartChoice_buttons, _leftContentElement)));
        _inLeftContent_buttons[1].clicked += () 
            => StartCoroutine(UITools.DelayedAction(() => ShowOptionPanel()));
        _inLeftContent_buttons[2].clicked += () => StartCoroutine(UITools.DelayedAction(() => ShowInformationPanel()));
        _inLeftContent_buttons[3].clicked += () => StartCoroutine(UITools.DelayedAction(() => QuitGame()));

        _inStartChoice_buttons[0].clicked += () => StartCoroutine(UITools.DelayedAction(() => StartGame()));
        _inStartChoice_buttons[1].clicked += () => StartCoroutine(UITools.DelayedAction(() => LoadGame()));
        _inStartChoice_buttons[2].clicked += () => StartCoroutine(UITools.DelayedAction(() => ClearContent(_inLeftContent_buttons, _leftContentElement)));

        _inGraphic_buttons[0].clicked += () => ExitPanelMode();

        _inVolume_buttons[0].clicked += () => ExitPanelMode();

        _inGuide_buttons[0].clicked += () => ExitPanelMode();

        _inMenu_buttons[0].clicked += () => StartCoroutine(UITools.DelayedAction(() => Continue()));
        _inMenu_buttons[1].clicked += () => StartCoroutine(UITools.DelayedAction(() => ResetGame()));
        _inMenu_buttons[2].clicked += () => StartCoroutine(UITools.DelayedAction(() => LoadGame()));
        _inMenu_buttons[3].clicked += () => StartCoroutine(UITools.DelayedAction(() => SaveGame()));
        _inMenu_buttons[4].clicked += () => StartCoroutine(UITools.DelayedAction(() => QuitGame()));
        _inMenu_buttons[5].clicked += () => StartCoroutine(UITools.DelayedAction(() => HideOptionPanel()));

        _inVolume_sliders[0].RegisterValueChangedCallback(evt => UITools.UpdateMusicVolume(evt.newValue, audioMixer));
        _inVolume_sliders[1].RegisterValueChangedCallback( evt => UITools.UpdateSFXVolume(evt.newValue, audioMixer));

        SetFocusPanelEvent();

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
            btn.RegisterCallback<FocusInEvent>(evt => ShowOnlyThisPanel(kvp.Value));
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
    #endregion

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
        if (!isPlay)
        {
            foreach (var btn in _inMenu_buttons)
            {
                if (btn.name == "backToNavBtn") continue;
                btn.style.display = DisplayStyle.None;
            }
        }
        else
        {
            foreach (var btn in _inMenu_buttons)
            {
                btn.style.display = DisplayStyle.Flex;
            }
        }
    }
    #endregion

    #region button action
    private void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    private void ShowInformationPanel()
    {
        Debug.Log("Information");
    }
    private void ShowOptionPanel()
    {
        _root.Clear();
        _root.Add(_optionElement);
    }
    private void HideOptionPanel()
    {
        _root.Clear();
        _root.Add(_container);

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
        Debug.Log("LoadGame");
    }
    private void SaveGame()
    {
        Debug.Log("SaveGame");
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    private void LoadData()
    {
        float currentVolume;
        if (audioMixer.GetFloat("MusicVolume", out currentVolume))
        {
            _inVolume_sliders[0].SetValueWithoutNotify(Mathf.Pow(10, currentVolume / 20f));
        }
        float sfx;
        if (audioMixer.GetFloat("SFXVolume", out sfx))
        {
            _inVolume_sliders[1].SetValueWithoutNotify(Mathf.Pow(10, sfx / 20f));
        }
    }
    private void ClearContent(List<Button> buttons, VisualElement element)
    {
        element.Clear();
        foreach (Button button in buttons)
        {
            _leftContentElement.Add(button);
        }
    }
    #endregion

}
