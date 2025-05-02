using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public GameObject itemPrefab;
    public string itemName;
    public Sprite icon;
    [TextArea(15, 20)]
    public string itemDescription;
    public bool stackable = true;
}
