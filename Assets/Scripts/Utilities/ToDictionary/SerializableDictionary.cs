using System.Collections.Generic;
[System.Serializable]
public class SerializableDictionary
{
    public List<SerializablePair> Dict = new List<SerializablePair>();
    public SerializableDictionary() { }

    public SerializableDictionary(Dictionary<string, bool> dict)
    {
        foreach (var kvp in dict)
        {
            Dict.Add(new SerializablePair { Key = kvp.Key, Value = kvp.Value });
        }
    }

    public Dictionary<string, bool> ToDictionary()
    {
        var dict = new Dictionary<string, bool>();
        foreach (var item in Dict)
        {
            dict[item.Key] = item.Value;
        }
        return dict;
    }
    public void Add(string key, bool value)
    {
        Dict.Add(new SerializablePair { Key = key, Value = value });
    }
    public bool TryGetValue(string key, out bool value)
    {
        foreach (var pair in Dict)
        {
            if (pair.Key == key)
            {
                value = pair.Value;
                return true;
            }
        }
        value = false;
        return false;
    }
    public bool ContainsKey(string key)
    {
        foreach (var pair in Dict)
        {
            if (pair.Key == key)
            {
                return true;
            }
        }
        return false;
    }
    public void Set(string key, bool value)
    {
        for (int i = 0; i < Dict.Count; i++)
        {
            if (Dict[i].Key == key)
            {
                Dict[i].Value = value;
                return;
            }
        }
        Dict.Add(new SerializablePair { Key = key, Value = value });
    }
}
[System.Serializable]
public class SerializablePair
{
    public string Key;
    public bool Value;
}
