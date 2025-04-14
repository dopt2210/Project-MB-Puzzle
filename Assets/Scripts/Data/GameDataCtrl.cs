using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataCtrl : MonoBehaviour
{
    public static GameDataCtrl Instance { get; private set; }

    [SerializeField] private string fileName;
    [SerializeField] private bool useEnDe;
    [SerializeField] private bool useInitData;

    private FileDataHandler fileHandler;
    private GameData gameData;
    private List<IGameData> gameDatas;
    private string gameDataPath = "SettingConf";

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        this.fileHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEnDe);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        this.gameDatas = FindAllGameData();
        LoadGame();
    }
    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }
    private List<IGameData> FindAllGameData()
    {
        IEnumerable<IGameData> gameDatas = FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.None).OfType<IGameData>();
        return new List<IGameData>(gameDatas);
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void SaveGame()
    {
        if (this.gameData == null)
        {
            Debug.LogWarning("No data found");
            return;
        }
        foreach (IGameData data in gameDatas)
        {
            data.SaveData(ref gameData);
        }
        fileHandler.Save(gameData, gameDataPath);
    }

    public void LoadGame()
    {
        this.gameData = fileHandler.Load(gameDataPath);

        if (this.gameData == null && useInitData)
        {
            NewGame();
        }

        if (gameData == null)
        {
            Debug.LogWarning("No data found");
            return;
        }
        foreach (IGameData data in gameDatas)
        {
            data.LoadData(gameData);
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    public bool HasGameData()
    {
        return gameData != null;
    }
    public Dictionary<string, GameData> GetData()
    {
        return fileHandler.GetDatas();
    }
}
