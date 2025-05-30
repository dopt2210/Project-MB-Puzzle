using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour, IGameData
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private UIDocument _document;
    [SerializeField] private OptionUI pauseSetup;

    List<Button> _inLeftContent_buttons = new List<Button>();
    List<Button> _inStartChoice_buttons = new List<Button>();

    private VisualElement _root, _container;

    private VisualElement _leftContentElement;
    private VisualElement _rightContentElement;

    private VisualElement _startElement;

    private float musicVolume, sfxVolume;
    private void Reset()
    {
        _document = GetComponent<UIDocument>();
        _audioMixer = Resources.Load<AudioMixer>("Master");
    }
    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;

        SetElement();
        pauseSetup.Initialize(_root, _audioMixer);
        SetEvent();
        SetButtonsEvent();

    }
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Start");

        // Set lại volume sau khi nhạc đã chạy
        UITools.UpdateMusicVolume(musicVolume, _audioMixer);
        UITools.UpdateSFXVolume(sfxVolume, _audioMixer);
    }
    #region button setup
    void SetButtonsEvent()
    {
        foreach (Button button in _inLeftContent_buttons)
        {
            button.clicked += ClickButton;

            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
        }
        foreach (Button button in _inStartChoice_buttons)
        {
            button.clicked += ClickButton;

            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
        }
    }
    void SetElement()
    {
        _container = _root.Q<VisualElement>("Container");

        _leftContentElement = _root.Q<VisualElement>("LeftContent");
        _rightContentElement = _root.Q<VisualElement>("RightContent").Q<Label>();
        _startElement = _root.Q<VisualElement>("StartChoice");

        _inLeftContent_buttons = _leftContentElement.Query<Button>().ToList();
        _inStartChoice_buttons = _startElement.Query<Button>().ToList();

    }
    void SetEvent()
    {
        _inLeftContent_buttons[0].clicked += () 
            => ClearContent(_inStartChoice_buttons, _leftContentElement);
        _inLeftContent_buttons[1].clicked += () 
            => ShowOptionPanel();
        _inLeftContent_buttons[2].clicked += () => ShowInformationPanel();
        _inLeftContent_buttons[3].clicked += () => QuitGame();

        _inStartChoice_buttons[0].clicked += () => NewGame();
        _inStartChoice_buttons[1].clicked += () => LoadGame();
        _inStartChoice_buttons[2].clicked += () => ClearContent(_inLeftContent_buttons, _leftContentElement);

        pauseSetup.OptionButtons[3].clicked += () => HideOptionPanel();
    }
    void ClickButton()
    {
        SoundManager.Instance.PlaySound2D("Click");
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

    #region button action
    private void NewGame()
    {
        GameDataManager.Instance.NewGame();
        StartGame();
    }
    private void StartGame()
    {
        SceneLoadManager.Instance.LoadSceneWithLoading("GameScene");
        MusicManager.Instance.PlayMusic("BGM");
    }
    private void ShowInformationPanel()
    {
        if (_rightContentElement.style.display == DisplayStyle.Flex)
        {
            _rightContentElement.style.display = DisplayStyle.None;
        }
        else
        {
            _rightContentElement.style.display = DisplayStyle.Flex;
        }
    }
    private void ShowOptionPanel()
    {
        _root.Clear();
        _root.Add(pauseSetup.OptionElement);
    }
    private void HideOptionPanel()
    {
        _root.Clear();
        _root.Add(_container);
    }
    private void LoadGame()
    {
        GameDataManager.Instance.LoadGame();
        StartGame();
    }
    private void QuitGame()
    {
        Application.Quit();
    }

    public void LoadData(GameData gameData)
    {
        musicVolume = gameData.musicVolume;
        sfxVolume = gameData.sfxVolume;
        pauseSetup._musicSlider?.SetValueWithoutNotify(musicVolume);
        pauseSetup._sfxSlider?.SetValueWithoutNotify(sfxVolume);
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.musicVolume = pauseSetup._musicSlider.value;
        gameData.sfxVolume = pauseSetup._sfxSlider.value;
    }


    #endregion

}