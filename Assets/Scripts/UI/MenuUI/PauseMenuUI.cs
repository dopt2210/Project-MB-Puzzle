using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class PauseMenuUI : MonoBehaviour, IGameData
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private UIDocument _document;
    [SerializeField] private OptionUI pauseSetup;

    List<Button> _inPauseContent_buttons = new List<Button>();

    private VisualElement _root, _container;
    private VisualElement _pauseElement;

    private float musicVolume, sfxVolume;

    private void Reset()
    {
        _document = GetComponent<UIDocument>();
        _audioMixer = Resources.Load<AudioMixer>("Master");
    }
    private void Awake()
    {
        _root = _document.rootVisualElement;

        SetElement();
        pauseSetup.Initialize(_root, _audioMixer);
        SetEvent();
        SetButtonsEvent();
    }
    private void Start()
    {
        UITools.UpdateMusicVolume(musicVolume, _audioMixer);
        UITools.UpdateSFXVolume(sfxVolume, _audioMixer);
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
            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
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
        _root.style.display = DisplayStyle.Flex;

    }
    public void Hide()
    {
        _root.style.display = DisplayStyle.None;

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
    private void Continue()
    {
        UIHandler.Instance.ResumeGame();
        Hide();
    }
    private void ResetGame()
    {
        Continue();
        GameManager.Instance.ResetMaze();
    }
    private void QuitGame()
    {
        GameDataManager.Instance.SaveGame();
        UIHandler.Instance.BackToMainMenu();
    }

    #endregion
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
}
