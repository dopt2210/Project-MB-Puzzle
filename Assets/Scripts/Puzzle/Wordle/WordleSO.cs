using UnityEngine;

[CreateAssetMenu(fileName = "WordleSO", menuName = "Scriptable Objects/WordleSO")]
public class WordleSO : ScriptableObject
{
    public string[] targetWords;

    public int wordLength = 5;

    public int rows = 6;

    private static readonly char[] ALPHABET =
    "abcdefghijklmnopqrstuvwxyz".ToCharArray();
    public string GetRandomWord()
    {
        if (targetWords != null && targetWords.Length > 0)
        {
            string w = targetWords[Random.Range(0, targetWords.Length)];
            return w.Trim().ToLower();
        }

        char[] buff = new char[wordLength];
        for (int i = 0; i < wordLength; i++)
            buff[i] = ALPHABET[Random.Range(0, ALPHABET.Length)];

        return new string(buff);
    }
}
