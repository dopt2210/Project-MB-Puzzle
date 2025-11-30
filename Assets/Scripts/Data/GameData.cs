[System.Serializable]
public class GameData
{
    public float musicVolume;
    public float sfxVolume;
    public SerializableDictionary puzzleStates;
    public int currentLevelIndex;
    public float timeUsed;
    public GameData()
    {
        musicVolume = 0;
        sfxVolume = 0;
        puzzleStates = new SerializableDictionary();
        currentLevelIndex = 0;
        timeUsed = 0f;
    }
}