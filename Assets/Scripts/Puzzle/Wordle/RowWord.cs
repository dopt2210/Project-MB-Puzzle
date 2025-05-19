using UnityEngine;

public class RowWord : MonoBehaviour
{
    public KeyWord[] cols { get; private set; }

    private void Awake()
    {
        cols = GetComponentsInChildren<KeyWord>();
    }
}
