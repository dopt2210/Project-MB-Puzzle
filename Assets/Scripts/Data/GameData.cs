using UnityEngine;

[System.Serializable]
public class GameData
{
    public int deathCount;
    public Vector3 playerPosition;
    public float musicVolume;
    public float sfxVolume;

    public GameData()
    {
        this.deathCount = 0;
        playerPosition = Vector3.zero;
        musicVolume = 0;
        sfxVolume = 0;
    }
}