using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private UIDocument _document;
    [SerializeField] private OptionUI pauseSetup;

    List<Button> _inLeftContent_buttons = new List<Button>();
    List<Button> _inStartChoice_buttons = new List<Button>();

    private VisualElement _root, _container;

    private VisualElement _leftContentElement;

    private VisualElement _startElement;

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
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Start");
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
        _startElement = _root.Q<VisualElement>("StartNotifyChoice");

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

        _inStartChoice_buttons[0].clicked += () => StartGame();
        _inStartChoice_buttons[1].clicked += () => LoadGame();
        _inStartChoice_buttons[2].clicked += () => ClearContent(_inLeftContent_buttons, _leftContentElement);

        pauseSetup.OptionButtons[3].clicked += () => HideOptionPanel();
    }
    #endregion
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

    #region button action
    private void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        MusicManager.Instance.PlayMusic("BGM");
        SceneLoadManager.instance.EnableLoading();

    }
    private void ShowInformationPanel()
    {
        Debug.Log("Information");
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
        Debug.Log("LoadGame");
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    //private void LoadData()
    //{
    //    float currentVolume;
    //    if (audioMixer.GetFloat("MusicVolume", out currentVolume))
    //    {
    //        _inVolume_sliders[0].SetValueWithoutNotify(Mathf.Pow(10, currentVolume / 20f));
    //    }
    //    float sfx;
    //    if (audioMixer.GetFloat("SFXVolume", out sfx))
    //    {
    //        _inVolume_sliders[1].SetValueWithoutNotify(Mathf.Pow(10, sfx / 20f));
    //    }
    //}
    
    #endregion

}
