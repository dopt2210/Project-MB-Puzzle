using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class FileDataHandler
{
    private string dataPath = "";
    private string dataName = "";
    private bool useEncryption = false;
    private readonly string encrypt = "JaggHeart";

    public FileDataHandler(string dataPath, string dataName, bool useEncryption)
    {
        this.dataPath = dataPath;
        this.dataName = dataName;
        this.useEncryption = useEncryption;
    }
    public GameData Load(string dataSlotId)
    {
        string fullPath = Path.Combine(dataPath, dataSlotId, dataName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (useEncryption)
                {
                    dataToLoad = EnDe(dataToLoad);  
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }

            catch (Exception e)
            {
                Debug.Log("Errror: " + fullPath + "\n" + e);

            }

        }
        return loadedData;
    }
    public void Save(GameData data, string dataSlotId)
    {
        string fullPath = Path.Combine(dataPath, dataSlotId, dataName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);
            if (useEncryption)
            {
                dataToStore = EnDe(dataToStore);
            }
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    public Dictionary<string, GameData> GetDatas()
    {
        Dictionary<string, GameData> dataDictionary = new Dictionary<string, GameData>();
        IEnumerable<DirectoryInfo> dataInfos = new DirectoryInfo(dataPath).EnumerateDirectories();
        foreach (DirectoryInfo directoryInfo in dataInfos)
        {
            string id = directoryInfo.Name;
            string fullPath = Path.Combine(dataPath, id, dataName);

            if (!File.Exists(fullPath))
            {
                continue;
            }

            GameData data = Load(id);

            if (data != null)
            {
                dataDictionary.Add(id, data);
            }
            else
            {
                 Debug.LogWarning("Error in " +  id);
            }
        }


        return dataDictionary;
    }
    
    private string EnDe(string data)
    {
        string modData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modData += (char)(data[i] ^ encrypt[i % encrypt.Length]);
        }
        return modData;
    }
}
