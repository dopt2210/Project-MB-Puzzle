using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour, IGameData
{  
    public static MenuManager instance {  get; private set; }

    #region UI Vars
    public GameObject CurrentUI { get; private set; }
    public GameObject PauseUI {  get; private set; }
    public GameObject PlayUI {  get; private set; }
    public GameObject SettingUI {  get; private set; }
    public GameObject ModePlayUI {  get; private set; }
    public GameObject SoundUI {  get; private set; }
    public GameObject PlayOptionUI { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Slider[] sliders;
    #endregion

    public static bool IsPaused { get; private set; }
    public static bool IsPlaying { get; private set; }

    private bool _isSwitchButtonDone;
    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        LoadComponent();
    }
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Start");
        CurrentUI = PlayUI;

        foreach (Transform transform in transform)
        {
            transform.gameObject.SetActive(false);
        }
        PlayUI.SetActive(true);

        SetButtonEvent();
        //if(!GameDataCtrl.Instance.HasGameData()) buttons[15].interactable = false;
    }
    private void Update()
    {
        SetButtonBack();
        if (IsPaused) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    #region Event 
    #region Button
    public void SetButtonEvent()
    {
        RemoveButtonEvent();

        //PlayUI
        buttons[0].onClick.AddListener(() => SwitchUI(PlayOptionUI));       //play
        buttons[1].onClick.AddListener(() => SwitchUI(SettingUI));          //option
        buttons[2].onClick.AddListener(() => QuitGame());                   //exit
        //PlayeOptionUI
        buttons[3].onClick.AddListener(() => NewGame());                    //new game
        buttons[4].onClick.AddListener(() => ContinueGame());               //continue
        buttons[5].onClick.AddListener(() => SwitchUI(PlayUI));             //backFromMenuPlay -> Play
        //SettingUI
        buttons[6].onClick.AddListener(() => SwitchUI(SoundUI));            //volume
        buttons[7].onClick.AddListener(() => SwitchUI(ModePlayUI));         //mode
        buttons[8].onClick.AddListener(() => SwitchUI(PlayUI));             //backFromSetting -> Play
        //ModeUI
        buttons[10].onClick.AddListener(() => SwitchUI(SoundUI));           //asian
        buttons[11].onClick.AddListener(() => SwitchUI(ModePlayUI));        //basic
        buttons[12].onClick.AddListener(() => SwitchUI(SettingUI));         //backFromMode -> Play
        //PauseUI
        //buttons[13].onClick.AddListener(() => PauseMenu.instance.Resume()); //resume
        buttons[14].onClick.AddListener(() => SwitchUI(SettingUI));         //option
        buttons[15].onClick.AddListener(() => BackToMainMenu());            //backOfPause - Menu
        //SoundUI
        sliders[0].onValueChanged.AddListener(UpdateMusicVolume);           //slider music
        sliders[1].onValueChanged.AddListener(UpdateSFXVolume);             //slider sound
        buttons[9].onClick.AddListener(() => SwitchUI(SettingUI));          //backOfSound - Option

        foreach (var button in buttons)
        {
            AddEventTrigger(button.gameObject, EventTriggerType.PointerEnter, OnPointerEnter);
            AddEventTrigger(button.gameObject, EventTriggerType.PointerClick, OnPointerClick);
        }
    }
    public void RemoveButtonEvent()
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }

        foreach (var slider in sliders)
        {
            RemoveAllEventTriggers(slider);
        }
    }
    #endregion
    #region Sound
    private void AddEventTrigger(GameObject target, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventType
        };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }
    private void RemoveAllEventTriggers(Slider slider)
    {
        EventTrigger trigger = slider.GetComponent<EventTrigger>();

        if (trigger != null)
        {
            trigger.triggers.Clear();
        }
    }
    private void OnPointerEnter(BaseEventData data)
    {
        SoundManager.Instance.PlaySound2D("Hover");
    }
    private void OnPointerClick(BaseEventData data)
    {
        SoundManager.Instance.PlaySound2D("Click");
    }
    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
    public void UpdateSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }
    #endregion
    #region UI
    private void SwitchUI(GameObject newUI)
    {
        if (CurrentUI != null)
        {
            CurrentUI.SetActive(false); 
        }
        CurrentUI = newUI; 
        CurrentUI.SetActive(true); 
    }
    #endregion
    #endregion

    #region Internal Function
    public static void SetPaused(bool value) => IsPaused = value;
    private void SetButtonBack()
    {
        if (_isSwitchButtonDone) return;
        if (!IsPlaying)
        {
            buttons[8].onClick.RemoveAllListeners();
            buttons[8].onClick.AddListener(() => SwitchUI(PlayUI));  //backOfOption - Play
        }
        else
        {
            buttons[8].onClick.RemoveAllListeners();
            buttons[8].onClick.AddListener(() => SwitchUI(PauseUI));  //backOfOption - Pause
        }
    }
    private void LoadComponent()
    {
        PlayUI = transform.GetChild(0).gameObject;
        PlayOptionUI = transform.GetChild(1).gameObject;
        SettingUI = transform.GetChild(2).gameObject;
        SoundUI = transform.GetChild(3).gameObject;
        ModePlayUI = transform.GetChild(4).gameObject;
        PauseUI = transform.GetChild(5).gameObject;

        buttons = GetComponentsInChildren<Button>();
    }

    private void ResetMenuOnLoad()
    {
        IsPlaying = false;
        IsPaused = false;

        _isSwitchButtonDone = false;
    }
    #endregion

    #region MenuAction
    public void PauseGame()
    {
        SwitchUI(PauseUI);
        IsPaused = true;
        InputManager.InputPlayer.SwitchCurrentActionMap("UI");
    }

    public void ResumeGame()
    {
        CurrentUI.SetActive(false);
        IsPaused = false;
        InputManager.InputPlayer.SwitchCurrentActionMap("Player");
    }
    private void NewGame()
    {
        CurrentUI.SetActive(false);
        MusicManager.Instance.PlayMusic("BGM");
        SceneManager.LoadScene("GameScene");
    }

    private void ContinueGame()
    {

        CurrentUI.SetActive(false);
        //PlayGameScene();

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SwitchToPlay()
    {
        SwitchUI(PlayUI);
    }
    public void BackToMainMenu()
    {
        //CurrentUI.SetActive(false);

        //ResetMenuOnLoad();

        //SceneLoadingCtrl.instance.EnableLoading();
        //SceneManager.LoadScene("MainMenu");
        //MusicManager.Instance.PlayMusic("MainMenu");
    }
    public void PlayStoryScene()
    {
        //SceneManager.LoadSceneAsync("StoryScene");
        //MusicManager.Instance.PlayMusic("Story");

        //GameDataCtrl.Instance.NewGame();
    }
    public void PlayGameScene()
    {
        //SceneLoadingCtrl.instance.EnableLoading();
        //SceneManager.LoadScene("GameScene");
        //MusicManager.Instance.PlayMusic("Theme");
        //IsPlaying = true;
        //_isSwitchButtonDone = false;
    }
    public void PlayNewGame()
    {
        //SceneLoadingCtrl.instance.EnableLoading();
        //MusicManager.Instance.PlayMusic("Theme");
        //IsPlaying = true;

    }
    #endregion

    #region Save

    public void LoadData(GameData gameData)
    {
        sliders[0].value = gameData.musicVolume;
        sliders[1].value = gameData.sfxVolume;
        UpdateMusicVolume(gameData.musicVolume);
        UpdateSFXVolume(gameData.sfxVolume);
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.musicVolume = sliders[0].value;
        gameData.sfxVolume = sliders[1].value;
    }
    #endregion
}
