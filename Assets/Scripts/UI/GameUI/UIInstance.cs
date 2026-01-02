using UnityEngine;
using UnityEngine.Audio;

public class UIInstance : MonoBehaviour, IGameData
{
    public static UIInstance Instance { get; private set; }
    [SerializeField] private UIOptionComponent optionPrefab;
    [SerializeField] private UIPause pausePrefab;
    [SerializeField] private UIMainMenu mainMenuPrefab;
    public static AudioMixer audioMixer { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);
        audioMixer = Resources.Load<AudioMixer>("Master");
    }
    private void Start()
    {
        MusicManager.Instance.PlayMusic("Start");
        QuickSetting();
            optionPrefab.gameObject.SetActive(false);
            pausePrefab.gameObject.SetActive(false);
            mainMenuPrefab.gameObject.SetActive(false);
        ShowMainMenuUI();

    }
    public void QuickSetting()
    {
        UITools.UpdateMusicVolume(optionPrefab.sliders[0].value, audioMixer); // Set default music volume
        UITools.UpdateSFXVolume(optionPrefab.sliders[1].value, audioMixer); // Set default SFX volume
    }

    public void ShowOptionUI()
    {
        optionPrefab.gameObject.SetActive(true);
    }
    public void HideOptionUI()
    {
        optionPrefab.gameObject.SetActive(false);
    }
    public void ShowPauseUI()
    {
        pausePrefab.gameObject.SetActive(true);
    }
    public void HidePauseUI()
    {
        pausePrefab.gameObject.SetActive(false);
    }
    public void ShowMainMenuUI()
    {
        mainMenuPrefab.gameObject.SetActive(true);
    }
    public void HideMainMenuUI()
    {
        mainMenuPrefab.gameObject.SetActive(false);
    }

    public void LoadData(GameData gameData)
    {
        var musicVolume = gameData.musicVolume;
        var sfxVolume = gameData.sfxVolume;
        optionPrefab.sliders[0]?.SetValueWithoutNotify(musicVolume);
        optionPrefab.sliders[1]?.SetValueWithoutNotify(sfxVolume);
        Debug.Log($"LoadData: Music Volume = {musicVolume}, SFX Volume = {sfxVolume}");
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.musicVolume = optionPrefab.sliders[0].value;
        gameData.sfxVolume = optionPrefab.sliders[1].value;
    }
}