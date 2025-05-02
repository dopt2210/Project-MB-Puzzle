using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CustomFocusRing : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private UIDocument _document;
    [SerializeField] private PauseSetup pauseSetup;

    List<Button> _inPauseContent_buttons = new List<Button>();

    private VisualElement _root, _container;
    private VisualElement _pauseElement;
    

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
        pauseSetup.Initialize(_root, audioMixer);
        SetEvent();
        SetButtonsEvent();
    }
    #region SetUp
    void SetElement()
    {
        _container = _root.Q<VisualElement>("Container");

        _pauseElement = _root.Q<VisualElement>("PauseContent");

        _inPauseContent_buttons = _pauseElement.Query<Button>().ToList();
    }
    void SetEvent()
    {
        _inPauseContent_buttons[0].clicked += () => Continue();
        _inPauseContent_buttons[1].clicked += () => ResetGame();
        _inPauseContent_buttons[2].clicked += () => ShowOptionPanel();
        _inPauseContent_buttons[3].clicked += () => QuitGame();

        pauseSetup.OptionButtons[3].clicked += () => HideOptionPanel();
    }
    void SetButtonsEvent()
    {
        foreach (var button in _inPauseContent_buttons)
        {
            button.clicked += ClickButton;
        }
        foreach (Button button in _inPauseContent_buttons)
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
    #endregion

    #region panel action
    public void Show()
    {
        gameObject.SetActive(true);
        FocusElement();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void FocusElement()
    {
        // Lấy phần tử muốn focus (ví dụ: nút đầu tiên trong menu)
        var firstButton = _root.Q<Button>("FisrtElement");

        if (firstButton != null)
        {
            // Dùng schedule để delay focus sau khi layout hoàn tất
            _root.schedule.Execute(() =>
            {
                firstButton.Focus();
            }).ExecuteLater(1); // Chờ 1 frame
        }
    }
    private void ShowOptionPanel()
    {
        _root.Clear();
        _root.Add(pauseSetup.OptionElement);
        FocusElement();
    }
    private void HideOptionPanel()
    {
        _root.Clear();
        _root.Add(_container);
        FocusElement();
    }
    private void Continue()
    {
        UIHandler.Instance.ResumeGame();
    }
    private void ResetGame()
    {
        Continue();
        GameManager.Instance.ResetMaze();
    }
    private void QuitGame()
    {
        UIHandler.Instance.BackToMainMenu();
    }
    #endregion
}
